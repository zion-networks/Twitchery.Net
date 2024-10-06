using Microsoft.Extensions.Logging;

namespace TwitcheryNet.Caching;

public class SmartCache<T> : ISmartCache where T : class, ICachable
{
    private readonly Dictionary<string, CachedValueData<T>> _cache = new();
    
    private readonly CacheFetchDelegate<T> _asyncFetcher;

    private SmartCachePool Pool { get; }
    private ILogger<SmartCache<T>> Logger { get; }

    public Type CacheType => typeof(T);

    public CachedValue<T>? this[string key] => GetOrFetchAsync(key).Result;

    public SmartCache(CacheFetchDelegate<T> asyncFetcher, SmartCachePool pool)
    {
        Logger = LoggerFactory
            .Create(x => x.AddConsole().SetMinimumLevel(LogLevel.Debug))
            .CreateLogger<SmartCache<T>>();
        
        Pool = pool;
        
        _asyncFetcher = asyncFetcher;
    }
    
    public async Task<CachedValue<T>?> GetOrFetchAsync(string key, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(key, out var cachedValue))
        {
            Logger.LogDebug("[SmartCache<{Type}>] Found {Key}", CacheType.Name, key);
            return cachedValue;
        }

        Logger.LogDebug("[SmartCache<{Type}>] Fetching {Key}", CacheType.Name, key);
        
        var fetchedValue = await _asyncFetcher(key, true, cancellationToken);
        if (fetchedValue is not null)
        {
            Logger.LogDebug("[SmartCache<{Type}>] Fetched {Key}", CacheType.Name, key);
            
            _cache[key] = new CachedValueData<T>(fetchedValue);
            return _cache[key];
        }
        
        Logger.LogWarning("[SmartCache<{Type}>] Failed to fetch {Key}", CacheType.Name, key);
        
        return null;
    }
    
    public Task ValidateAsync()
    {
        foreach (var (key, value) in _cache)
        {
            if (value.IsExpired)
            {
                var relativeLastAccess = (long)(value.Created - value.LastAccessed).TotalSeconds;
                var percentageLastAccess = relativeLastAccess / value.Lifespan; // last access percentage of lifespan
                
                // If the value has never been accessed or has been last accessed less than 50% of its lifespan, remove it
                if (value.HasActiveEventListeners is false && (value.AccessCount == 0 || percentageLastAccess < 0.5))
                {
                    Logger.LogDebug("[SmartCache<{Type}>] Removing {Key}", CacheType.Name, key);
                    _cache.Remove(key);
                }
                else
                {
                    Logger.LogDebug("[SmartCache<{Type}>] Invalidating {Key}", CacheType.Name, key);
                    value.IsInvalidated = true;
                }
            }
        }

        return Task.CompletedTask;
    }

    public async Task RefreshAsync()
    {
        foreach (var (key, cachedValue) in _cache)
        {
            if (cachedValue.IsInvalidated)
            {
                Logger.LogDebug("[SmartCache<{Type}>] Refreshing {Key}", CacheType.Name, key);
                
                var fetchedValue = await _asyncFetcher(key, false);
                if (fetchedValue is not null)
                {
                    cachedValue.Refresh(fetchedValue);
                    Logger.LogDebug("[SmartCache<{Type}>] Refreshed {Key}", CacheType.Name, key);
                }
                else
                {
                    Logger.LogWarning("[SmartCache<{Type}>] Failed to refresh {Key}", CacheType.Name, key);
                }
            }
        }
    }
}