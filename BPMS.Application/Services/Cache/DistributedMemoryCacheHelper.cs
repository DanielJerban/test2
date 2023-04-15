using BPMS.Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;

namespace BPMS.Application.Services.Cache;

public class DistributedMemoryCacheHelper : IDistributedCacheHelper
{
    private static readonly MemoryCache MemoryCache = new(new MemoryCacheOptions());

    public T GetObject<T>(string cacheKey)
    {
        T answer = default;

        object cacheValue = MemoryCache.Get(cacheKey);

        if (cacheValue != null)
        {
            answer = (T)cacheValue;
        }

        return answer;
    }

    public string GetString(string cacheKey)
    {
        return (string)MemoryCache.Get(cacheKey);
    }

    public void SetString(string cacheKey, string cacheValue, TimeSpan expirationTime)
    {
        var expiration = DateTime.Now.Add(expirationTime);
        MemoryCache.Set(cacheKey, cacheValue, expiration);
    }

    public void SetObject<T>(string cacheKey, T cacheValue, TimeSpan expirationTime)
    {
        MemoryCache.Set(cacheKey, cacheValue, DateTime.Now.Add(expirationTime));
    }

    public T GetOrSet<T>(string cacheKey, Func<T> callbackFunction, TimeSpan expirationTime)
    {
        T answer;

        object cacheValue = MemoryCache.Get(cacheKey);

        if (cacheValue != null)
        {
            answer = (T)cacheValue;
            return answer;
        }

        answer = callbackFunction();

        if (answer != null)
        {
            cacheValue = answer;

            MemoryCache.Set(cacheKey, cacheValue, DateTime.Now.Add(expirationTime));
        }

        return answer;
    }

    public void Remove(string cacheKey)
    {
        MemoryCache.Remove(cacheKey);
    }
}