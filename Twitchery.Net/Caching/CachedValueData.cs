using System.Reflection;
using Microsoft.Extensions.Logging;
using TwitcheryNet.Caching.Attributes;
using TwitcheryNet.Extensions;

namespace TwitcheryNet.Caching;

public class CachedValueData<T> : CachedValue<T> where T : class, ICachable
{
    private T _value;
    public DateTime Created { get; }
    public DateTime Refreshed { get; private set; }
    public DateTime Expiration { get; private set; }
    public DateTime LastAccessed { get; private set; }
    public long AccessCount { get; private set; }
    public long RefreshCount { get; private set; }
    public long Lifespan { get; }
    public bool IsInvalidated { get; set; }
    
    private ILogger<CachedValueData<T>> Logger { get; } = LoggerFactory
        .Create(x => x.AddConsole().SetMinimumLevel(LogLevel.Debug))
        .CreateLogger<CachedValueData<T>>();

    public override T Value
    {
        get
        {
            LastAccessed = DateTime.UtcNow;
            AccessCount++;
            
            return _value;
        }
        protected set
        {
            Refreshed = DateTime.UtcNow;
            RefreshCount++;
            
            _value = value;
        }
    }

    public bool IsExpired => DateTime.UtcNow > Expiration;

    public bool HasActiveEventListeners => typeof(T)
        .GetEvents()
        .Select(e => typeof(T).GetField(e.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField))
        .Select(f => f?.GetValue(_value))
        .Where(v => v is not null)
        .Cast<Delegate>()
        .SelectMany(d => d.GetInvocationList())
        .Any();
    
    public CachedValueData(T value, DateTime? expiration = null)
    {
        _value = value;
        Created = DateTime.UtcNow;
        LastAccessed = DateTime.UtcNow;
        
        if (expiration is null && typeof(T).TryGetCustomAttribute(out CachingAttribute cachingAttribute))
        {
            Expiration = DateTime.UtcNow.AddSeconds(cachingAttribute.CacheDuration);
            Lifespan = cachingAttribute.CacheDuration;
            
            Logger.LogDebug("Cached value for {Type} has been created with a lifespan of {Lifespan} seconds", typeof(T).Name, Lifespan);
        }
        else
        {
            Expiration = expiration ?? DateTime.MaxValue;
            Lifespan = (long)(Expiration - LastAccessed).TotalSeconds;
            
            Logger.LogDebug("Cached value for {Type} has been created with a lifespan of {Lifespan} seconds", typeof(T).Name, Lifespan);
        }
    }

    public void Refresh(T fetchedValue)
    {
        var tProps = typeof(T).GetProperties();
        foreach (var prop in tProps)
        {
            var isNoCaching = prop.HasCustomAttribute<NoCaching>();
            var isRetain = prop.HasCustomAttribute<CacheRetainAttribute>();
            var isKeyProp = prop.HasCustomAttribute<CachingKeyAttribute>();
            
            var newValue = prop.GetValue(fetchedValue);
            if (isNoCaching || isRetain || isKeyProp)
            {
                continue;
            }
            
            prop.SetValue(_value, newValue);
        }
        
        Expiration = DateTime.UtcNow.AddSeconds(Lifespan);
        Refreshed = DateTime.UtcNow;
        IsInvalidated = false;
    }
    
    public static implicit operator T(CachedValueData<T> c) => c.Value;
    public static implicit operator CachedValueData<T>(T v) => new(v);
}