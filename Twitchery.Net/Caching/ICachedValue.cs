namespace TwitcheryNet.Caching;

public interface ICachedValue<out T> where T : class, ICachable
{
    T Value { get; }
}