using System.Runtime.CompilerServices;
using TwitcheryNet.Attributes;
using TwitcheryNet.Models.Helix;
using TwitcheryNet.Models.Indexer;

namespace TwitcheryNet.Services.Interfaces;

public interface ITwitchApiService
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? ClientAccessToken { get; set; }
    public List<string> ClientScopes { get; }
    
    public UsersIndex Users { get; }
    public StreamsIndex Streams { get; }
    public ChannelInformationsIndex Channels { get; }
    public ChannelFollowersIndex ChannelFollowers { get; }

    string GetOAuthUrl(string redirectUri, string[] scopes, string? state = null);
    
    Task<bool> StartBrowserUserAuthentication(string redirectUri, string[] scopes);
    
    ApiRoute PreTwitchApiCall(Type callerType, string callerMemberName);
    
    Task<TResponse?> GetTwitchApiAsync<TQuery, TResponse>(TQuery? query, Type callerType, CancellationToken token = default,
        [CallerMemberName] string? callerMemberName = null)
        where TQuery : class, IQueryParameters
        where TResponse : class;

    Task<TResponse?> PostTwitchApiAsync<TQuery, TBody, TResponse>(TQuery? query, TBody? body, Type callerType,
        CancellationToken token = default, [CallerMemberName] string? callerMemberName = null)
        where TQuery : class, IQueryParameters
        where TBody : class
        where TResponse : class;

    Task<TResponse?> PostTwitchApiAsync<TBody, TResponse>(TBody? body, Type callerType, CancellationToken token = default,
        [CallerMemberName] string? callerMemberName = null)
        where TBody : class
        where TResponse : class;

    Task<TFullResponse> GetTwitchApiAllAsync<TQuery, TResponse, TFullResponse>(TQuery? query, Type callerType, CancellationToken token = default, [CallerMemberName] string? callerMemberName = null)
        where TQuery : class, IQueryParameters, IWithPagination
        where TResponse : class, IHasPagination
        where TFullResponse : class, IHasTotal, IFullResponse<TResponse>, new();
}