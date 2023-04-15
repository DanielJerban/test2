using BPMS.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace BPMS.Application.Services.Cache;

public class DistributedRedisCacheHelper : IDistributedCacheHelper
{
    private readonly IDatabase _redis;

    public DistributedRedisCacheHelper(IConfiguration configuration)
    {
        var redisConnection = ConnectionMultiplexer.Connect(configuration.GetConnectionString("BPMS-Redis") ?? throw new NullReferenceException("Invalid connection string"));
        _redis = redisConnection.GetDatabase();
    }

    public string GetString(string cacheKey)
    {
        string fixedCacheKey = getCacheKeyWithPrefix(cacheKey);
        return _redis.StringGet(fixedCacheKey);
    }

    public void SetString(string cacheKey, string cacheValue, TimeSpan expirationTime)
    {
        string fixedCacheKey = getCacheKeyWithPrefix(cacheKey);
        _redis.StringSet(fixedCacheKey, cacheValue, expirationTime);
    }

    public void Remove(string cacheKey)
    {
        string fixedCacheKey = getCacheKeyWithPrefix(cacheKey);
        _redis.KeyDelete(fixedCacheKey);
    }

    public T GetObject<T>(string cacheKey)
    {
        string strData = GetString(cacheKey);
        if (string.IsNullOrEmpty(strData))
            return default;

        return JsonConvert.DeserializeObject<T>(strData);
    }

    public void SetObject<T>(string cacheKey, T cacheValue, TimeSpan expirationTime)
    {
        string strObject = JsonConvert.SerializeObject(cacheValue);
        SetString(cacheKey, strObject, expirationTime);
    }

    public T GetOrSet<T>(string cacheKey, Func<T> callbackFunction, TimeSpan expirationTime)
    {
        T data = GetObject<T>(cacheKey);
        if (data != null)
            return data;

        data = callbackFunction();
        SetObject(cacheKey, data, expirationTime);
        return data;
    }

    private string getCacheKeyWithPrefix(string cacheKey) => $"BPMS_{cacheKey}";
}