namespace BPMS.Infrastructure.Services;

public interface IDistributedCacheHelper
{
    T GetObject<T>(string cacheKey);
    string GetString(string cacheKey);
    void SetString(string cacheKey, string cacheValue, TimeSpan expirationTime);
    void SetObject<T>(string cacheKey, T cacheValue, TimeSpan expirationTime);
    T GetOrSet<T>(string cacheKey, Func<T> callbackFunction, TimeSpan expirationTime);
    void Remove(string cacheKey);
}