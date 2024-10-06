using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TwitcheryNet.Attributes;
using TwitcheryNet.Caching;
using TwitcheryNet.Exceptions;
using TwitcheryNet.Extensions;
using TwitcheryNet.Http;
using TwitcheryNet.Misc;
using TwitcheryNet.Models.Auth.Flow;
using TwitcheryNet.Models.Helix;
using TwitcheryNet.Models.Helix.Users;
using TwitcheryNet.Models.Indexer;
using TwitcheryNet.Models.OAuth2;
using TwitcheryNet.Models.OAuth2.Validation;
using TwitcheryNet.Net.EventSub;
using TwitcheryNet.Services.Interfaces;

namespace TwitcheryNet.Services.Implementations;

public class Twitchery : ITwitchery, IAsyncDisposable
{
    #region Private Properties
    
    private string? UserLogin { get; set; }
    
    #endregion Private Properties
    
    #region Public Properties

    public string? UserClientId { get; set; }
    public string? UserAccessToken { get; set; }
    public List<string> UserScopes { get; private set; } = [];
    
    public string? AppClientId { get; set; }
    public string? AppClientSecret { get; set; }
    public string? AppAccessToken { get; set; }
    public List<string> AppClientScopes { get; set; } = [];

    #endregion

    #region Internal Properties

    EventSubClient ITwitchery.EventSubClient { get; set; }
    SmartCachePool ITwitchery.SmartCachePool { get; set; }

    #endregion Internal Properties

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
    public ChannelsIndex Channels => new(this);
    public ModerationIndex Moderation => new(this);
    public PollsIndex Polls => new(this);
    
    #endregion Indexed Properties

    #region Shorthand Properties

    public User? Me => string.IsNullOrEmpty(UserLogin) ? null : Users[UserLogin]?.Value;
    
    public bool HasUserToken => !string.IsNullOrWhiteSpace(UserAccessToken);
    public bool HasAppToken => !string.IsNullOrWhiteSpace(AppAccessToken);
    public bool HasAnyToken => HasUserToken || HasAppToken;

    #endregion Shorthand Properties
    
    public Twitchery()
    {
        Logger = LoggerFactory.Create(config =>
        {
            config.AddConsole().SetMinimumLevel(LogLevel.Debug);
        }).CreateLogger<Twitchery>();
        
        ((ITwitchery)this).EventSubClient = new(this);
        ((ITwitchery)this).SmartCachePool = new(this);
    }

    [ActivatorUtilitiesConstructor]
    public Twitchery(ILogger<Twitchery> logger)
    {
        Logger = logger;
        
        ((ITwitchery)this).EventSubClient = new(this);
        ((ITwitchery)this).SmartCachePool = new(this);
    }
    
    public string GetOAuthUrl(string redirectUri, string[] scopes, string? state = null)
    {
        var scope = string.Join("+", scopes);
        
        var url = $"{TwitchImplicitGrantUrl}" +
               $"?client_id={UserClientId}" +
               $"&redirect_uri={redirectUri}" +
               $"&response_type=token" +
               $"&scope={scope}";
        
        if (state is not null)
        {
            url += $"&state={state}";
        }

        return url;
    }
    
    public async Task<bool> UserBrowserAuthAsync(string clientId, string redirectUri, params string[] scopes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clientId, nameof(clientId));
        ArgumentException.ThrowIfNullOrWhiteSpace(redirectUri, nameof(redirectUri));
        
        if (scopes.Length == 0)
        {
            throw new ArgumentException("At least one scope is required.");
        }
        
        UserClientId = clientId;
        
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
        
        UserAccessToken = oauthLogin.AccessToken;
        UserScopes = (oauthLogin.Scope?.Split('+') ?? [])
            .Select(Uri.UnescapeDataString)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();

        var me = await Users.GetUsersAsync(new GetUsersRequest(), timeOutCancellation.Token);
        UserLogin = me?.Users.FirstOrDefault()?.Login;

        if (string.IsNullOrWhiteSpace(UserLogin))
        {
            Logger.LogError("Failed to retrieve user login.");
            return false;
        }
        
        return true;
    }
    
    public async Task<bool> AppAuthAsync(string clientId, string clientSecret, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clientId, nameof(clientId));
        ArgumentException.ThrowIfNullOrWhiteSpace(clientSecret, nameof(clientSecret));
        
        AppClientId = clientId;
        AppClientSecret = clientSecret;

        var request = new ClientCredentialsFlowRequest(clientId, clientSecret);
        var response = await AsyncHttpClient
            .StartPost("https://id.twitch.tv/oauth2/token")
            .SetContentType(MediaTypeNames.Application.FormUrlEncoded)
            .SetQuery(request)
            .Build()
            .SendAsync<ClientCredentialsFlowResponse>(token);
        
        if (response.Validate("https://id.twitch.tv/oauth2/validate") is false)
        {
            return false;
        }
        
        if (response.Body is null)
        {
            return false;
        }
        
        AppAccessToken = response.Body.AccessToken;
        
        return true;
    }

    public async Task<bool> CheckAppToken()
    {
        if (string.IsNullOrWhiteSpace(AppAccessToken))
        {
            return false;
        }

        var request = new ValidateOauth2TokenRequest();
        var response = await AsyncHttpClient
            .StartGet("https://id.twitch.tv/oauth2/validate")
            .AddHeader("Authorization", "OAuth " + AppAccessToken)
            .SetQuery(request)
            .Build()
            .SendAsync<ValidateOAuth2TokenResponse>();
        
        if (response.Validate("https://id.twitch.tv/oauth2/validate") is false)
        {
            return false;
        }
        
        if (response.Body is null)
        {
            return false;
        }
        
        if (response.Body.ClientId != AppClientId)
        {
            Logger.LogWarning("Client ID mismatch on App Token validation.");
            return false;
        }
        
        return response.Body?.ExpiresIn > 0;
    }
    
    public async Task<bool> CheckUserToken()
    {
        if (string.IsNullOrWhiteSpace(UserAccessToken))
        {
            return false;
        }

        var request = new ValidateOauth2TokenRequest();
        var response = await AsyncHttpClient
            .StartGet("https://id.twitch.tv/oauth2/validate")
            .AddHeader("Authorization", "OAuth " + UserAccessToken)
            .SetQuery(request)
            .Build()
            .SendAsync<ValidateOAuth2TokenResponse>();

        if (response.Validate("https://id.twitch.tv/oauth2/validate") is false)
        {
            return false;
        }
        
        if (response.Body is null)
        {
            return false;
        }
        
        if (response.Body.ClientId != UserClientId)
        {
            Logger.LogWarning("Client ID mismatch on App Token validation.");
            return false;
        }
        
        return response.Body?.ExpiresIn > 0;
    }

    private Route GetRoute(Type callerType, string callerMemberName)
    {
        ArgumentException.ThrowIfNullOrEmpty(callerMemberName, nameof(callerMemberName));
        
        if (string.IsNullOrWhiteSpace(UserClientId))
        {
            throw new ApiException("Client ID is required.");
        }
        
        if (string.IsNullOrWhiteSpace(UserAccessToken))
        {
            throw new ApiException("Access Token is required.");
        }

        var apiMethod = callerType
            .GetMethods()
            .Where(m => m.Name.Equals(callerMemberName))
            .FirstOrDefault(m => m.HasCustomAttribute<ApiRoute>());

        if (apiMethod is null)
        {
            throw new MissingMethodException(callerType.FullName, callerMemberName);
        }

        var apiRoute = apiMethod.GetCustomAttribute<ApiRoute>();
        var apiRules = apiMethod.GetCustomAttribute<ApiRules>();
        var requiredToken = apiMethod.GetCustomAttribute<RequiresTokenAttribute>()?.TokenType;

        if (requiredToken is null)
        {
            throw new MissingAttributeException<RequiresTokenAttribute>(apiMethod);
        }
        
        if (apiRoute is null)
        {
            throw new MissingAttributeException<ApiRoute>(apiMethod);
        }

        var route = new Route(TwitchApiEndpoint, apiRoute, apiMethod, requiredToken.Value, apiRules);
        
        return route;
    }

    public OAuthCredentials GetCredentialsForRoute(Route route)
    {
        ArgumentNullException.ThrowIfNull(route, nameof(route));

        if (route.RequiredTokenType == TokenType.UserAccess)
        {
            if (string.IsNullOrWhiteSpace(UserClientId))
            {
                throw new ApiException("User Client ID is required.");
            }
            
            if (string.IsNullOrWhiteSpace(UserAccessToken))
            {
                throw new ApiException("User Access Token is required.");
            }
        }

        if (route.RequiredTokenType == TokenType.AppAccess)
        {
            if (string.IsNullOrWhiteSpace(AppClientId))
            {
                throw new ApiException("App Client ID is required.");
            }
            
            if (string.IsNullOrWhiteSpace(AppClientSecret))
            {
                throw new ApiException("App Client Secret is required.");
            }
            
            if (string.IsNullOrWhiteSpace(AppAccessToken))
            {
                throw new ApiException("App Access Token is required.");
            }
        }
        
        if (route.RequiredTokenType == TokenType.Both)
        {
            if (string.IsNullOrWhiteSpace(UserClientId) && string.IsNullOrWhiteSpace(AppClientId))
            {
                throw new ApiException("Either User or App Client ID is required.");
            }
            
            if (string.IsNullOrWhiteSpace(UserAccessToken) && string.IsNullOrWhiteSpace(AppAccessToken))
            {
                throw new ApiException("Either User or App Access Token is required.");
            }
        }

        return route.RequiredTokenType switch
        {
            TokenType.UserAccess => new OAuthCredentials(UserClientId!, UserAccessToken!),
            TokenType.AppAccess => new OAuthCredentials(AppClientId!, AppClientSecret!, AppAccessToken!),
            TokenType.Both => UserAccessToken is not null
                ? new OAuthCredentials(UserClientId!, UserAccessToken!)
                : new OAuthCredentials(AppClientId!, AppClientSecret!, AppAccessToken!),
            _ => throw new ArgumentOutOfRangeException(nameof(route.RequiredTokenType), route.RequiredTokenType, "Invalid token type.")
        };
    }
    
    private List<ValidationResult> ValidateRoute(Route route, [CallerMemberName] string? callerMemberName = null)
    {
        var results = new List<ValidationResult>();

        if (callerMemberName?.StartsWith(route.ApiRoute.HttpMethod, StringComparison.CurrentCultureIgnoreCase) is false)
        {
            results.Add(new ValidationResult($"Invalid HTTP method for route {route.ApiRoute.Path}"));
        }
        
        foreach (var scope in route.ApiRoute.RequiredScopes)
        {
            if (UserScopes.Contains(scope) is false)
            {
                results.Add(new ValidationResult($"Missing required scope {scope} on route {route.ApiRoute.Path}"));
            }
        }

        if (Uri.IsWellFormedUriString(route.FullUrl, UriKind.Absolute) is false)
        {
            results.Add(new ValidationResult($"Invalid API route URL: {route.ApiRoute.Path}"));
        }
        
        switch (route.RequiredTokenType)
        {
            case TokenType.UserAccess when HasUserToken is false:
                results.Add(new ValidationResult("User Access Token is required."));
                break;
            
            case TokenType.AppAccess when HasAppToken is false:
                results.Add(new ValidationResult("App Access Token is required."));
                break;
            
            case TokenType.Both when HasAnyToken is false:
                results.Add(new ValidationResult("User or App Access Token is required."));
                break;
        }

        // TODO: Implement rules validation

        return results;
    }

    public async Task<TResponse?> GetTwitchApiAsync<TQuery, TResponse>(TQuery? query, Type callerType, CancellationToken token = default, [CallerMemberName] string? callerMemberName = null)
        where TQuery : class, IQueryParameters
        where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(query, nameof(query));
        ArgumentException.ThrowIfNullOrEmpty(callerMemberName, nameof(callerMemberName));
        
        var route = GetRoute(callerType, callerMemberName);

        var validationResults = ValidateRoute(route);
        if (validationResults.Count != 0)
        {
            throw new ApiException($"Route validation failed:\n{string.Join("\n- ", validationResults)}");
        }
        
        var credentials = GetCredentialsForRoute(route);
        var result = await AsyncHttpClient
            .StartGet(route.FullUrl)
            .AddHeader("Authorization", $"Bearer {credentials.AccessToken}")
            .AddHeader("Client-Id", credentials.ClientId)
            .SetQuery(query)
            .Build()
            .SendAsync<TResponse>(token);
        
        if (result.Validate(route.FullUrl, route.ApiRoute.RequiredStatusCode) is false)
        {
            return null;
        }

        return result.Body;
    }
    
    public async Task<TFullResponse> GetTwitchApiAllAsync<TQuery, TResponse, TFullResponse>(TQuery? query, Type callerType, CancellationToken token = default, [CallerMemberName] string? callerMemberName = null)
        where TQuery : class, IQueryParameters, IWithPagination
        where TResponse : class, IHasPagination
        where TFullResponse : class, IHasTotal, IFullResponse<TResponse>, new()
    {
        ArgumentNullException.ThrowIfNull(query, nameof(query));
        ArgumentException.ThrowIfNullOrEmpty(callerMemberName, nameof(callerMemberName));
        
        var route = GetRoute(callerType, callerMemberName);
        
        var validationResults = ValidateRoute(route);
        if (validationResults.Count != 0)
        {
            throw new ApiException($"Route validation failed:\n{string.Join("\n- ", validationResults)}");
        }

        var responses = new TFullResponse();
        string? after = null;
        do
        {
            if (query is IWithPagination pagination)
            {
                pagination.After = after;
            }
            
            var credentials = GetCredentialsForRoute(route);
            var result = await AsyncHttpClient
                .StartGet(route.FullUrl)
                .AddHeader("Authorization", $"Bearer {credentials.AccessToken}")
                .AddHeader("Client-Id", credentials.ClientId)
                .SetQuery(query)
                .Build()
                .SendAsync<TResponse>(token);
            
            if (result.Validate(route.FullUrl, route.ApiRoute.RequiredStatusCode) is false)
            {
                return new TFullResponse();
            }
            
            var response = result.Body;
            
            if (response is null)
            {
                continue;
            }

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
        
        var route = GetRoute(callerType, callerMemberName);
        
        var validationResults = ValidateRoute(route);
        if (validationResults.Count != 0)
        {
            throw new ApiException($"Route validation failed:\n{string.Join("\n- ", validationResults)}");
        }

        var credentials = GetCredentialsForRoute(route);
        var result = await AsyncHttpClient
            .StartPost(route.FullUrl)
            .AddHeader("Authorization", $"Bearer {credentials.AccessToken}")
            .AddHeader("Client-Id", credentials.ClientId)
            .SetQuery(query)
            .SetBody(body)
            .Build()
            .SendAsync<TResponse>(token);
        
        if (result.Validate(route.FullUrl, route.ApiRoute.RequiredStatusCode) is false)
        {
            return null;
        }

        return result.Body;
    }
    
    public async Task<TResponse?> PostTwitchApiAsync<TBody, TResponse>(TBody? body, Type callerType, CancellationToken token = default, [CallerMemberName] string? callerMemberName = null)
        where TBody : class
        where TResponse : class
    {
        ArgumentException.ThrowIfNullOrEmpty(callerMemberName, nameof(callerMemberName));
        
        var route = GetRoute(callerType, callerMemberName);
        
        var validationResults = ValidateRoute(route);
        if (validationResults.Count != 0)
        {
            throw new ApiException($"Route validation failed:\n{string.Join("\n- ", validationResults)}");
        }

        var credentials = GetCredentialsForRoute(route);
        var result = await AsyncHttpClient
            .StartPost(route.FullUrl)
            .AddHeader("Authorization", $"Bearer {credentials.AccessToken}")
            .AddHeader("Client-Id", credentials.ClientId)
            .SetBody(body)
            .Build()
            .SendAsync<TResponse>(token);
        
        if (result.Validate(route.FullUrl, route.ApiRoute.RequiredStatusCode) is false)
        {
            return null;
        }

        return result.Body;
    }
    
    public async Task PostTwitchApiAsync<TQuery>(TQuery? query, Type callerType, CancellationToken token = default, [CallerMemberName] string? callerMemberName = null)
        where TQuery : class, IQueryParameters
    {
        ArgumentException.ThrowIfNullOrEmpty(callerMemberName, nameof(callerMemberName));
        
        var route = GetRoute(callerType, callerMemberName);
        
        var validationResults = ValidateRoute(route);
        if (validationResults.Count != 0)
        {
            throw new ApiException($"Route validation failed:\n{string.Join("\n- ", validationResults)}");
        }

        var credentials = GetCredentialsForRoute(route);
        var response = await AsyncHttpClient
            .StartPost(route.FullUrl)
            .AddHeader("Authorization", $"Bearer {credentials.AccessToken}")
            .AddHeader("Client-Id", credentials.ClientId)
            .SetQuery(query)
            .Build()
            .SendAsync(token);

        response.Validate(route.FullUrl, route.ApiRoute.RequiredStatusCode);
    }

    public async Task InjectDataAsync<TTarget>(TTarget target, CancellationToken token = default) where TTarget : class
    {
        var type = typeof(TTarget);
        var properties = type.GetProperties();

        if (target is IHasTwitchery twitcheryTarget)
        {
            twitcheryTarget.Twitch = this;
        }
        
        foreach (var prop in properties)
        {
            var injectRouteData = prop.GetCustomAttribute<InjectRouteData>();
            
            if (injectRouteData is null)
            {
                continue;
            }

            var propClass = prop.DeclaringType;
            var propType = prop.PropertyType;
            var sourceType = injectRouteData.SourceType;
            var sourceMethodName = injectRouteData.SourceMethodName;

            if (propClass is null)
            {
                Logger.LogWarning($"Injection Data Resolve Warning: Property {prop.Name} has no declaring type.");
                continue;
            }
            
            Logger.LogDebug("Injecting data to {PropName} : {PropType} of class {ClassFullName}", prop.Name, propType.FullName, propClass.FullName);

            var targetMethod = sourceType
                .GetMethods()
                .Where(m => m.Name.Equals(sourceMethodName))
                .Where(m =>
                {
                    var mParams = m.GetParameters();
                    return mParams.Length >= 1 && mParams[0].ParameterType == propClass;
                }).FirstOrDefault();

            if (targetMethod is null)
            {
                Logger.LogWarning($"Injection Data Resolve Warning: No method found for {sourceType.FullName}.{sourceMethodName} " +
                                  $"with parameter {propClass.Name}.");
                continue;
            }
            
            var instanceProperty = GetType()
                .GetProperties()
                .FirstOrDefault(p => p.PropertyType == sourceType);

            if (instanceProperty is null)
            {
                Logger.LogWarning("No source provided by Twitchery for requested data type {SourceType}.", sourceType.FullName);
                continue;
            }
            
            var instanceValue = instanceProperty.GetValue(this);
            
            if (instanceValue is null)
            {
                Logger.LogWarning("Injection Data Resolve Warning: Source instance {SourceType} is null.", sourceType.FullName);
                continue;
            }

            var propertyTypeAsTask = typeof(Task<>).MakeGenericType(prop.PropertyType); // User => Task<User>
            var typeMatches = prop.PropertyType == targetMethod.ReturnType; // User == User
            var typeMatchesGenericTask = propertyTypeAsTask == targetMethod.ReturnType; // Task<User> == Task<User>
            
            if (typeMatches is false && typeMatchesGenericTask is false)
            {
                throw new TargetInvocationException($"Injection Data Resolve Error: " +
                                                    $"Invalid data type {prop.PropertyType} for " +
                                                    $"{propClass.Name}.{prop.Name} - injecting method requires the " +
                                                    $"property type to be {targetMethod.ReturnType} or " +
                                                    $"{propertyTypeAsTask}", null);
            }

            if (targetMethod.ReturnType.IsGenericType &&
                targetMethod.ReturnType.GetGenericArguments().FirstOrDefault() == typeof(void))
            {
                throw new TargetInvocationException($"Injection Data Resolve Error: " +
                                                    $"Invalid method return type for {sourceType.FullName}.{sourceMethodName} " +
                                                    $"while resolving {propClass.Name}.{prop.Name}: void is not allowed", null);
            }

            var targetParams = targetMethod.GetParameters();
            
            var invokationParams = new object[targetParams.Length];
            if (targetParams.Length == 2 && targetParams[1].ParameterType == typeof(CancellationToken))
            {
                invokationParams[0] = target;
                invokationParams[1] = token;
            }
            else if (targetParams.Length == 1)
            {
                invokationParams[0] = target;
            }
            else
            {
                throw new TargetInvocationException($"Injection Data Resolve Error: Invalid method signature for " +
                                                    $"{sourceType.FullName}.{sourceMethodName} while resolving " +
                                                    $"{propClass.Name}.{prop.Name}", null);
            }

            var rawResult = targetMethod.Invoke(instanceValue, invokationParams);

            if (rawResult is null)
            {
                Logger.LogWarning($"Injection Data Resolve Warning: No data returned for {sourceType.FullName}.{sourceMethodName} " +
                                  $"while resolving {propClass.Name}.{prop.Name}.");
                continue;
            }

            if (targetMethod.ReturnType == propertyTypeAsTask)
            {
                var awaitableTask = (dynamic) rawResult;
                var awaitedResult = await awaitableTask;

                prop.SetValue(target, awaitedResult);
            }
            else
            {
                prop.SetValue(target, rawResult);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await ((ITwitchery)this).SmartCachePool.DisposeAsync();
    }
}