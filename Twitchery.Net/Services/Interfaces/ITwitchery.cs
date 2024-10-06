using System.Runtime.CompilerServices;
using TwitcheryNet.Caching;
using TwitcheryNet.Models.Helix;
using TwitcheryNet.Models.Helix.Users;
using TwitcheryNet.Models.Indexer;
using TwitcheryNet.Net.EventSub;

namespace TwitcheryNet.Services.Interfaces;

public interface ITwitchery
{
    public string? UserClientId { get; set; }
    public string? UserAccessToken { get; set; }
    public List<string> UserScopes { get; }
    
    public string? AppClientId { get; set; }
    public string? AppClientSecret { get; set; }
    public string? AppAccessToken { get; set; }
    public List<string> AppClientScopes { get; set; }
    
    internal EventSubClient EventSubClient { get; set; }
    internal SmartCachePool SmartCachePool { get; set; }
    
    public UsersIndex Users { get; }
    public StreamsIndex Streams { get; }
    public ChannelsIndex Channels { get; }
    public ChatIndex Chat { get; }
    public User? Me { get; }
    bool HasUserToken { get; }
    bool HasAppToken { get; }
    bool HasAnyToken { get; }

    string GetOAuthUrl(string redirectUri, string[] scopes, string? state = null);
    Task<bool> UserBrowserAuthAsync(string clientId, string redirectUri, string[] scopes);
    Task<bool> AppAuthAsync(string clientId, string clientSecret, CancellationToken token = default);
    Task<bool> CheckAppToken();
    Task<bool> CheckUserToken();
    
    Task<TResponse?> GetTwitchApiAsync<TQuery, TResponse>(TQuery? query, Type callerType, CancellationToken token = default,
        [CallerMemberName] string? callerMemberName = null)
        where TQuery : class, IQueryParameters
        where TResponse : class;

    Task<TFullResponse> GetTwitchApiAllAsync<TQuery, TResponse, TFullResponse>(TQuery? query, Type callerType, CancellationToken token = default, [CallerMemberName] string? callerMemberName = null)
        where TQuery : class, IQueryParameters, IWithPagination
        where TResponse : class, IHasPagination
        where TFullResponse : class, IHasTotal, IFullResponse<TResponse>, new();

    Task<TResponse?> PostTwitchApiAsync<TQuery, TBody, TResponse>(TQuery? query, TBody? body, Type callerType,
        CancellationToken token = default, [CallerMemberName] string? callerMemberName = null)
        where TQuery : class, IQueryParameters
        where TBody : class
        where TResponse : class;

    Task<TResponse?> PostTwitchApiAsync<TBody, TResponse>(TBody? body, Type callerType, CancellationToken token = default,
        [CallerMemberName] string? callerMemberName = null)
        where TBody : class
        where TResponse : class;

    Task InjectDataAsync<TResponse>(TResponse target, CancellationToken token = default) where TResponse : class;
}