using System.Runtime.CompilerServices;
using TwitcheryNet.Attributes;
using TwitcheryNet.Models.Helix;

namespace TwitcheryNet.Services.Interfaces;

public interface ITwitchApiService
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? AccessToken { get; set; }
    public List<string> Scopes { get; }

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

    Task<bool> StartImplicitAuthenticationAsync(string redirectUri, string[] scopes);
    
    ApiRoute PreTwitchApiCall(Type callerType, string callerMemberName);
}