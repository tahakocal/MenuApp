namespace MenuApp.Core.Cache;

public interface IMemoryCacheService
{
    public T TryGetOrCreate<T>(string key, Func<T> createItemCallback, TimeSpan? expirationTime = null);
    public void RemoveAllCaches();

    public void RemoveByPrefix(string key);
}