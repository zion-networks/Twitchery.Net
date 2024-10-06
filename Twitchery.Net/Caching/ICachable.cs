namespace TwitcheryNet.Caching;

public interface ICachable
{
    public string GetCacheKey();
}