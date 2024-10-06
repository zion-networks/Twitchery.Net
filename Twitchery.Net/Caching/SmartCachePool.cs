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
    
    public SmartCache<T> GetOrCreateCache<T>(CacheFetchDelegate<T> asyncFetcher) where T : class, ICachable
    {
        Logger.LogDebug("Getting or creating cache for {Type}...", typeof(T).Name);
        
        if (_cachePool.TryGetValue(typeof(T), out var cache))
        {
            return (SmartCache<T>) cache;
        }
        
        var smartCache = new SmartCache<T>(asyncFetcher, this);
        
        _cachePool.Add(typeof(T), smartCache);
        
        return smartCache;
    }

    private void ValidateCache(object? state)
    {
        foreach (var (type, cache) in _cachePool)
        {
            Logger.LogDebug("Validating cache for {Type}...", type.Name);

            cache.ValidateAsync().Wait();
            
            Logger.LogDebug("Refreshing cache for {Type}...", type.Name);
            
            cache.RefreshAsync().Wait();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _validationTask.DisposeAsync();
    }
}