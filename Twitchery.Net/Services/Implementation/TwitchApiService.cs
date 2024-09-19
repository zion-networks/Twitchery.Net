using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TwitcheryNet.Attributes;
using TwitcheryNet.Exceptions;
using TwitcheryNet.Http;
using TwitcheryNet.Misc;
using TwitcheryNet.Models.Helix;
using TwitcheryNet.Models.Helix.Channels;
using TwitcheryNet.Models.Helix.Chat;
using TwitcheryNet.Models.Helix.Streams;
using TwitcheryNet.Services.Interfaces;

namespace TwitcheryNet.Services.Implementation;

public class TwitchApiService : ITwitchApiService
{
    #region Public Properties

    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? AccessToken { get; set; }
    public List<string> Scopes { get; private set; } = [];

    #endregion

    #region Services
    
    private ILogger<TwitchApiService> Logger { get; }
    
    #endregion
    
    #region Private Constants
    
    private const string TwitchImplicitGrantUrl = "https://id.twitch.tv/oauth2/authorize";
    private const string TwitchApiEndpoint = "https://api.twitch.tv/helix/";
    
    #endregion
    
    public TwitchApiService()
    {
        Logger = LoggerFactory.Create(config =>
        {
            config.AddConsole();
        }).CreateLogger<TwitchApiService>();
    }

    [ActivatorUtilitiesConstructor]
    public TwitchApiService(ILogger<TwitchApiService> logger)
    {
        Logger = logger;
    }
    
    public async Task<bool> StartImplicitAuthenticationAsync(string redirectUri, string[] scopes)
    {
        var state = Guid.NewGuid().ToString();
        var scope = string.Join("+", scopes);
        var url = $"{TwitchImplicitGrantUrl}" +
                  $"?client_id={ClientId}" +
                  $"&redirect_uri={redirectUri}" +
                  $"&response_type=token" +
                  $"&scope={scope}" +
                  $"&state={state}";
        
        var oauthServer = new OAuthHttpServer(redirectUri.EndsWith('/') ? redirectUri : $"{redirectUri}/", state);
        
        WebTools.OpenUrl(url);
        
        var timeOutCancellation = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var oauthLogin = await oauthServer.WaitForAuthentication(timeOutCancellation.Token);
        
        if (oauthLogin is null)
        {
            Logger.LogError("Failed to authenticate with Twitch.");
            return false;
        }
        
        AccessToken = oauthLogin.AccessToken;
        Scopes = (oauthLogin.Scope?.Split('+') ?? [])
            .Select(Uri.UnescapeDataString)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList()!;
        
        return true;
    }

    private ApiRoute PreTwitchApiCall(string callerMemberName)
    {
        ArgumentException.ThrowIfNullOrEmpty(callerMemberName, nameof(callerMemberName));
        
        if (string.IsNullOrWhiteSpace(ClientId))
        {
            throw new ApiException("Client ID is required.");
        }
        
        if (string.IsNullOrWhiteSpace(AccessToken))
        {
            throw new ApiException("Access Token is required.");
        }

        var method = GetType().GetMethod(callerMemberName) ?? throw new MissingMethodException(GetType().FullName, callerMemberName);
        var routeAttribute = method.GetCustomAttribute<ApiRoute>() ?? throw new Exception("No ApiRoute Attribute");
        var requiredScopes = routeAttribute.RequiredScopes;
        
        MissingTwitchScopeException.ThrowIfMissing(Scopes, requiredScopes);
        
        var apiFullRoute = $"{TwitchApiEndpoint}{routeAttribute.Route}";
        
        if (Uri.IsWellFormedUriString(apiFullRoute, UriKind.Absolute) == false)
        {
            throw new UriFormatException($"Invalid API route URL: {apiFullRoute}");
        }
        
        return routeAttribute;
    }

    private async Task<TResponse?> GetTwitchApiAsync<TQuery, TResponse>(TQuery? query, CancellationToken token = default, [CallerMemberName] string? callerMemberName = null)
        where TQuery : class, IQueryParameters
        where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(query, nameof(query));
        ArgumentException.ThrowIfNullOrEmpty(callerMemberName, nameof(callerMemberName));
        
        var routeAttribute = PreTwitchApiCall(callerMemberName);
        var apiFullRoute = $"{TwitchApiEndpoint}{routeAttribute.Route}";

        if (routeAttribute.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase) is false)
        {
            throw new ApiException("Only GET requests are supported.");
        }

        var result = await AsyncHttpClient
            .StartGet(apiFullRoute)
            .AddHeader("Authorization", $"Bearer {AccessToken}")
            .AddHeader("Client-Id", ClientId!)
            .SetQueryString(query)
            .RequireStatusCode(routeAttribute.RequiredStatusCode)
            .Build()
            .SendAsync<TResponse>(token);

        return result.Body;
    }
    
    private async Task<TResponse?> PostTwitchApiAsync<TQuery, TBody, TResponse>(TQuery? query, TBody? body, CancellationToken token = default, [CallerMemberName] string? callerMemberName = null)
        where TQuery : class, IQueryParameters
        where TBody : class
        where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(query, nameof(query));
        ArgumentException.ThrowIfNullOrEmpty(callerMemberName, nameof(callerMemberName));
        
        var routeAttribute = PreTwitchApiCall(callerMemberName);
        var apiFullRoute = $"{TwitchApiEndpoint}{routeAttribute.Route}";

        if (routeAttribute.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase) is false)
        {
            throw new ApiException("Only POST requests are supported.");
        }

        var result = await AsyncHttpClient
            .StartPost(apiFullRoute)
            .AddHeader("Authorization", $"Bearer {AccessToken}")
            .AddHeader("Client-Id", ClientId!)
            .SetQueryString(query)
            .SetBody(body)
            .RequireStatusCode(routeAttribute.RequiredStatusCode)
            .Build()
            .SendAsync<TResponse>(token);

        return result.Body;
    }
    
    [ApiRoute("GET", "chat/chatters", "moderator:read:chatters")]
    public async Task<GetChattersResponse?> GetChattersAsync(string broadcasterId, string moderatorId, int? first = null, string? after = null, CancellationToken cancellationToken = default)
    {
        var request = new GetChattersRequest(broadcasterId, moderatorId)
        {
            First = first,
            After = after
        };
        
        return await GetTwitchApiAsync<GetChattersRequest, GetChattersResponse>(request, cancellationToken);
    }
    
    [ApiRoute("GET", "channels/followers", "moderator:read:followers")]
    public async Task<GetChannelFollowersResponse?> GetChannelFollowersAsync(string broadcasterId, string? userId = null, int? first = null, string? after = null, CancellationToken cancellationToken = default)
    {
        var request = new GetChannelFollowersRequest(broadcasterId)
        {
            UserId = userId,
            First = first,
            After = after
        };
        
        return await GetTwitchApiAsync<GetChannelFollowersRequest, GetChannelFollowersResponse>(request, cancellationToken);
    }
    
    [ApiRoute("GET", "streams")]
    public async Task<GetStreamsResponse?> GetStreamsAsync(
        List<string>? userIds = null,
        List<string>? userLogins = null,
        List<string>? gameIds = null,
        string? type = null,
        List<string>? languages = null,
        int? first = null,
        string? before = null,
        string? after = null,
        CancellationToken cancellationToken = default)
    {
        var request = new GetStreamsRequest
        {
            UserId = userIds,
            UserLogin = userLogins,
            GameId = gameIds,
            Type = type,
            Language = languages,
            First = first,
            Before = before,
            After = after
        };
        
        return await GetTwitchApiAsync<GetStreamsRequest, GetStreamsResponse>(request, cancellationToken);
    }
    
    [ApiRoute("GET", "channels")]
    public async Task<GetChannelInformationResponse?> GetChannelInformationAsync(string broadcasterId, CancellationToken cancellationToken = default)
    {
        var request = new GetChannelInformationRequest(broadcasterId);
        
        return await GetTwitchApiAsync<GetChannelInformationRequest, GetChannelInformationResponse>(request, cancellationToken);
    }
}