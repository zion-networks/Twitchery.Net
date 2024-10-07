using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;
using TwitcheryNet.Services.Interfaces;

namespace TwitcheryNet.Caching;

public class SmartCachePool : IAsyncDisposable
{
    private readonly Dictionary<Type, ISmartCache> _cachePool = new();
    private readonly Timer _validationTask;
    
    private ITwitchery Twitchery { get; }
    private ILogger<SmartCachePool> Logger { get; }
    
    public SmartCachePool(ITwitchery twitchery)
    {
        Logger = LoggerFactory
            .Create(x => x.AddConsole().SetMinimumLevel(LogLevel.Debug))
            .CreateLogger<SmartCachePool>();
        
        Twitchery = twitchery;
        
        _validationTask = new Timer(ValidateCache, null, 0, Milliseconds.Seconds15);
    }

    [ActivatorUtilitiesConstructor]
    public SmartCachePool(ILogger<SmartCachePool> logger, ITwitchery twitchery) : this(twitchery)
    {
        Logger = logger;
    }
    
    public CachedValue<T>? Get<T>(string key) where T : class, ICachable
    {
        var t = typeof(T);
        
        if (_cachePool.TryGetValue(t, out var cache))
        {
            return ((SmartCache<T>) cache)[key];
        }
        
        return null;
    }
    
    public SmartCache<T> GetOrCreateCache<T>(CacheFetchDelegate<T> asyncFetcher) where T : class, ICachable
    {
        if (_cachePool.TryGetValue(typeof(T), out var cache))
        {
            Logger.LogDebug("Found existing cache for {Type}...", typeof(T).Name);
            return (SmartCache<T>) cache;
        }
        
        Logger.LogDebug("Creating new cache for {Type}...", typeof(T).Name);
        
        var smartCache = new SmartCache<T>(asyncFetcher, this);
        _cachePool.Add(typeof(T), smartCache);
        
        return smartCache;
    }

    private void ValidateCache(object? state)
    {
        Logger.LogDebug("Validating and refreshing caches ...");
        foreach (var (_, cache) in _cachePool)
        {
            cache.ValidateAsync().Wait();
            cache.RefreshAsync().Wait();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _validationTask.DisposeAsync();
    }
}