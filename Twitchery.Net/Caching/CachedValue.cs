namespace TwitcheryNet.Caching;

public abstract class CachedValue<T> : ICachedValue<T> where T : class, ICachable
{
    public abstract T Value { get; protected set; }
    
    public static implicit operator T(CachedValue<T> cachedValue) => cachedValue.Value;
}