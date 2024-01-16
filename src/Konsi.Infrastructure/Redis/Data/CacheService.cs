using StackExchange.Redis;
using System.Text.Json;

namespace Konsi.Infrastructure.Redis.Data;

public class CacheService
{
    private readonly IConnectionMultiplexer _redisConnection;

    public CacheService(IConnectionMultiplexer redisConnection)
    {
        _redisConnection = redisConnection;
    }

    public async Task<string> GetCachedDataAsync(string key)
    {
        IDatabase db = _redisConnection.GetDatabase();
        return await db.StringGetAsync(key);
    }

    public async Task CacheDataAsync(string key, object data)
    {
        IDatabase db = _redisConnection.GetDatabase();
        string jsonData = JsonSerializer.Serialize(data);
        bool success = await db.StringSetAsync(key, jsonData);

        if (!success)
        {
            throw new Exception("Falha ao salvar os dados no cache do Redis.");
        }
    }
}
