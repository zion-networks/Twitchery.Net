using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TwitcheryNet.Attributes;
using TwitcheryNet.Exceptions;
using TwitcheryNet.Http;
using TwitcheryNet.Misc;
using TwitcheryNet.Models.Helix;
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

    public ApiRoute PreTwitchApiCall(Type callerType, string callerMemberName)
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

        var method = callerType.GetMethod(callerMemberName) ?? throw new MissingMethodException(callerType.FullName, callerMemberName);
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
            .AddHeader("Authorization", $"Bearer {AccessToken}")
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
                .AddHeader("Authorization", $"Bearer {AccessToken}")
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
            .AddHeader("Authorization", $"Bearer {AccessToken}")
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
            .AddHeader("Authorization", $"Bearer {AccessToken}")
            .AddHeader("Client-Id", ClientId!)
            .SetBody(body)
            .RequireStatusCode(routeAttribute.RequiredStatusCode)
            .Build()
            .SendAsync<TResponse>(token);

        return result.Body;
    }
}