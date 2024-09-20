using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TwitcheryNet.Attributes;
using TwitcheryNet.Exceptions;
using TwitcheryNet.Http;
using TwitcheryNet.Misc;
using TwitcheryNet.Models.Helix;
using TwitcheryNet.Models.Helix.Users;
using TwitcheryNet.Models.Indexer;
using TwitcheryNet.Services.Interfaces;

namespace TwitcheryNet.Services.Implementations;

public class Twitchery : ITwitchery
{
    #region Public Properties

    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? ClientAccessToken { get; set; }
    public List<string> ClientScopes { get; private set; } = [];

    #endregion

    #region Services
    
    private ILogger<Twitchery> Logger { get; }
    
    #endregion
    
    #region Private Constants
    
    private const string TwitchImplicitGrantUrl = "https://id.twitch.tv/oauth2/authorize";
    private const string TwitchApiEndpoint = "https://api.twitch.tv/helix/";
    
    #endregion
    
    #region Indexed Properties

    public UsersIndex Users => new(this);
    public StreamsIndex Streams => new(this);
    public ChatIndex Chat => new(this);
    public ChannelInformationsIndex Channels => new(this);
    public ChannelFollowersIndex ChannelFollowers => new(this);
    
    #endregion Indexed Properties

    #region Shorthand Properties

    public User? Me => Users.GetUsersAsync(new GetUsersRequest()).Result?.Users.FirstOrDefault();

    #endregion Shorthand Properties
    
    public Twitchery()
    {
        Logger = LoggerFactory.Create(config =>
        {
            config.AddConsole();
        }).CreateLogger<Twitchery>();
    }

    [ActivatorUtilitiesConstructor]
    public Twitchery(ILogger<Twitchery> logger)
    {
        Logger = logger;
    }
    
    public string GetOAuthUrl(string redirectUri, string[] scopes, string? state = null)
    {
        var scope = string.Join("+", scopes);
        
        var url = $"{TwitchImplicitGrantUrl}" +
               $"?client_id={ClientId}" +
               $"&redirect_uri={redirectUri}" +
               $"&response_type=token" +
               $"&scope={scope}";
        
        if (state is not null)
            url += $"&state={state}";
        
        return url;
    }
    
    public async Task<bool> UserBrowserAuthAsync(string redirectUri, params string[] scopes)
    {
        if (scopes.Length == 0)
        {
            throw new ArgumentException("At least one scope is required.");
        }
        
        var state = Guid.NewGuid().ToString();
        var url = GetOAuthUrl(redirectUri, scopes, state);
        
        var oauthServer = new OAuthHttpServer(redirectUri.EndsWith('/') ? redirectUri : $"{redirectUri}/", state);
        
        WebTools.OpenUrl(url);
        
        var timeOutCancellation = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var oauthLogin = await oauthServer.WaitForAuthentication(timeOutCancellation.Token);
        
        if (oauthLogin is null)
        {
            Logger.LogError("Failed to authenticate with Twitch.");
            return false;
        }
        
        ClientAccessToken = oauthLogin.AccessToken;
        ClientScopes = (oauthLogin.Scope?.Split('+') ?? [])
            .Select(Uri.UnescapeDataString)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();
        
        return true;
    }

    public ApiRoute PreTwitchApiCall(Type callerType, string callerMemberName)
    {
        ArgumentException.ThrowIfNullOrEmpty(callerMemberName, nameof(callerMemberName));
        
        if (string.IsNullOrWhiteSpace(ClientId))
        {
            throw new ApiException("Client ID is required.");
        }
        
        if (string.IsNullOrWhiteSpace(ClientAccessToken))
        {
            throw new ApiException("Access Token is required.");
        }
        
        var method = callerType
            .GetMethods()
            .Where(m => m.Name.Equals(callerMemberName))
            .FirstOrDefault(m => m.GetCustomAttribute<ApiRoute>() is not null) ?? throw new MissingMethodException(callerType.FullName, callerMemberName);

        var routeAttribute = method.GetCustomAttribute<ApiRoute>() ?? throw new Exception("No ApiRoute Attribute");
        var requiredScopes = routeAttribute.RequiredScopes;
        
        MissingTwitchScopeException.ThrowIfMissing(ClientScopes, requiredScopes);
        
        var apiFullRoute = $"{TwitchApiEndpoint}{routeAttribute.Route}";
        
        if (Uri.IsWellFormedUriString(apiFullRoute, UriKind.Absolute) == false)
        {
            throw new UriFormatException($"Invalid API route URL: {apiFullRoute}");
        }
        
        return routeAttribute;
    }

    public async Task<TResponse?> GetTwitchApiAsync<TQuery, TResponse>(TQuery? query, Type callerType, CancellationToken token = default, [CallerMemberName] string? callerMemberName = null)
        where TQuery : class, IQueryParameters
        where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(query, nameof(query));
        ArgumentException.ThrowIfNullOrEmpty(callerMemberName, nameof(callerMemberName));
        
        var routeAttribute = PreTwitchApiCall(callerType, callerMemberName);
        var apiFullRoute = $"{TwitchApiEndpoint}{routeAttribute.Route}";

        if (routeAttribute.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase) is false)
        {
            throw new ApiException("Only GET requests are supported.");
        }

        var result = await AsyncHttpClient
            .StartGet(apiFullRoute)
            .AddHeader("Authorization", $"Bearer {ClientAccessToken}")
            .AddHeader("Client-Id", ClientId!)
            .SetQueryString(query)
            .RequireStatusCode(routeAttribute.RequiredStatusCode)
            .Build()
            .SendAsync<TResponse>(token);

        return result.Body;
    }
    
    public async Task<TFullResponse> GetTwitchApiAllAsync<TQuery, TResponse, TFullResponse>(TQuery? query, Type callerType, CancellationToken token = default, [CallerMemberName] string? callerMemberName = null)
        where TQuery : class, IQueryParameters, IWithPagination
        where TResponse : class, IHasPagination
        where TFullResponse : class, IHasTotal, IFullResponse<TResponse>, new()
    {
        ArgumentNullException.ThrowIfNull(query, nameof(query));
        ArgumentException.ThrowIfNullOrEmpty(callerMemberName, nameof(callerMemberName));
        
        var routeAttribute = PreTwitchApiCall(callerType, callerMemberName);
        var apiFullRoute = $"{TwitchApiEndpoint}{routeAttribute.Route}";

        if (routeAttribute.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase) is false)
        {
            throw new ApiException("Only GET requests are supported.");
        }
        
        var responses = new TFullResponse();
        string? after = null;
        do
        {
            if (query is IWithPagination pagination)
            {
                pagination.After = after;
            }
            
            var result = await AsyncHttpClient
                .StartGet(apiFullRoute)
                .AddHeader("Authorization", $"Bearer {ClientAccessToken}")
                .AddHeader("Client-Id", ClientId!)
                .SetQueryString(query)
                .RequireStatusCode(routeAttribute.RequiredStatusCode)
                .Build()
                .SendAsync<TResponse>(token);
            
            var response = result.Body;
            
            if (response is null)
                continue;
            
            responses.Add(response);
            
            after = response.Pagination.Cursor;
        } while (after is not null);
        
        return responses;
    }
    
    public async Task<TResponse?> PostTwitchApiAsync<TQuery, TBody, TResponse>(TQuery? query, TBody? body, Type callerType, CancellationToken token = default, [CallerMemberName] string? callerMemberName = null)
        where TQuery : class, IQueryParameters
        where TBody : class
        where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(query, nameof(query));
        ArgumentException.ThrowIfNullOrEmpty(callerMemberName, nameof(callerMemberName));
        
        var routeAttribute = PreTwitchApiCall(callerType, callerMemberName);
        var apiFullRoute = $"{TwitchApiEndpoint}{routeAttribute.Route}";

        if (routeAttribute.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase) is false)
        {
            throw new ApiException("Only POST requests are supported.");
        }

        var result = await AsyncHttpClient
            .StartPost(apiFullRoute)
            .AddHeader("Authorization", $"Bearer {ClientAccessToken}")
            .AddHeader("Client-Id", ClientId!)
            .SetQueryString(query)
            .SetBody(body)
            .RequireStatusCode(routeAttribute.RequiredStatusCode)
            .Build()
            .SendAsync<TResponse>(token);

        return result.Body;
    }
    
    public async Task<TResponse?> PostTwitchApiAsync<TBody, TResponse>(TBody? body, Type callerType, CancellationToken token = default, [CallerMemberName] string? callerMemberName = null)
        where TBody : class
        where TResponse : class
    {
        ArgumentException.ThrowIfNullOrEmpty(callerMemberName, nameof(callerMemberName));
        
        var routeAttribute = PreTwitchApiCall(callerType, callerMemberName);
        var apiFullRoute = $"{TwitchApiEndpoint}{routeAttribute.Route}";

        if (routeAttribute.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase) is false)
        {
            throw new ApiException("Only POST requests are supported.");
        }

        var result = await AsyncHttpClient
            .StartPost(apiFullRoute)
            .AddHeader("Authorization", $"Bearer {ClientAccessToken}")
            .AddHeader("Client-Id", ClientId!)
            .SetBody(body)
            .RequireStatusCode(routeAttribute.RequiredStatusCode)
            .Build()
            .SendAsync<TResponse>(token);

        return result.Body;
    }
}