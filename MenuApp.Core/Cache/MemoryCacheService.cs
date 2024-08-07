using System.Collections;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;

namespace MenuApp.Core.Cache;

public class MemoryCacheService : IMemoryCacheService
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public T TryGetOrCreate<T>(string key, Func<T> createItemCallback, TimeSpan? expirationTime = null)
    {
        if (_memoryCache.TryGetValue(key, out T value)) return value;

        //TODO: decide default expirationTime
        expirationTime ??= TimeSpan.FromDays(3);
        var createdValue = createItemCallback();
        Set(key, createdValue, expirationTime.Value);

        return createdValue;
    }

    public void RemoveAllCaches()
    {
        var cacheKeys = GetAllCacheKeys();
        foreach (var key in cacheKeys) _memoryCache.Remove(key);
    }

    public void RemoveByPrefix(string key)
    {
        _memoryCache.Remove(key);
    }

    private void Set<T>(string key, T value, TimeSpan expirationTime)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expirationTime
        };

        _memoryCache.Set(key, value, cacheEntryOptions);
    }

    private IEnumerable<string> GetAllCacheKeys()
    {
        var cacheField = _memoryCache.GetType().GetField("_entries", BindingFlags.NonPublic | BindingFlags.Instance);
        var cacheEntries = cacheField?.GetValue(_memoryCache) as IDictionary;

        if (cacheEntries != null)
            foreach (DictionaryEntry cacheItem in cacheEntries)
                yield return (string)cacheItem.Key;
    }
}