namespace TwitcheryNet.Caching;

public delegate Task<T?> CacheFetchDelegate<T>(string key, bool noInject, CancellationToken cancellationToken = default) where T : class, ICachable;