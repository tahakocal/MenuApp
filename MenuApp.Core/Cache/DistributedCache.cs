using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace MenuApp.Core.Cache;

public static class RedisCache
{
    public static Task SetAsync<T>(this IDistributedCache distributedCache, string key, T entry,
        TimeSpan? expirationTime = null)
    {
        var options = new DistributedCacheEntryOptions();

        if (expirationTime.HasValue) options.SetAbsoluteExpiration(expirationTime.Value);

        var jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve
        };

        return distributedCache.SetStringAsync(key, JsonSerializer.Serialize(entry, jsonOptions), options);
    }

    public static async Task<T> GetAsync<T>(this IDistributedCache distributedCache, string key)
    {
        var stored = await distributedCache.GetStringAsync(key);

        var jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve
        };

        if (!string.IsNullOrEmpty(stored)) return JsonSerializer.Deserialize<T>(stored, jsonOptions);

        return default;
    }

    public static async Task<TItem> GetOrCreateAsync<TItem>(this IDistributedCache cache, string key,
        Func<Task<TItem>> func)
    {
        var result = await cache.GetAsync<TItem>(key);

        if (result == null)
        {
            result = await func();
            //cache 7 days 
            var expirationDate = TimeSpan.FromDays(7);

            await cache.SetAsync(key, result, expirationDate);
        }

        return result;
    }

    public static async Task<bool> RemoveAllAsync(this IDistributedCache distributedCache, IConfiguration configuration)
    {
        try
        {
            var redisConn = configuration.GetValue<string>("RedisCacheUrl");
            var redis = ConnectionMultiplexer.Connect($"{redisConn},allowAdmin=true");
            var server = redis.GetServer(redisConn);
            await server.FlushDatabaseAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static async Task RemoveByPrefix(this IDistributedCache cache, string key)
    {
        await cache.RemoveAsync(key);
    }
}