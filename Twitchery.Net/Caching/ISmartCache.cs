namespace TwitcheryNet.Caching;

public interface ISmartCache
{
    public Type CacheType { get; }
    public Task ValidateAsync();
    public Task RefreshAsync();
}