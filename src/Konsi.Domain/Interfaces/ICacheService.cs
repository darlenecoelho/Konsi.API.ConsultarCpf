namespace Konsi.Domain.Interfaces;

public interface ICacheService
{
    Task<string> GetCachedDataAsync(string key);
    Task CacheDataAsync(string key, object data);
}
