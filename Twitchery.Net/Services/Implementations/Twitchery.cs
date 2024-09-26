using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TwitcheryNet.Attributes;
using TwitcheryNet.Exceptions;
using TwitcheryNet.Extensions;
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
    public ChannelsIndex Channels => new(this);
    public ModerationIndex Moderation => new(this);
    public PollsIndex Polls => new(this);
    
    #endregion Indexed Properties

    #region Shorthand Properties

    public User? Me { get; private set; }

    #endregion Shorthand Properties
    
    public Twitchery()
    {
        Logger = LoggerFactory.Create(config =>
        {
            config.AddConsole().SetMinimumLevel(LogLevel.Debug);
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

        var me = await Users.GetUsersAsync(new GetUsersRequest(), timeOutCancellation.Token);
        Me = me?.Users.FirstOrDefault();
        
        return true;
    }

    private Route GetRoute(Type callerType, string callerMemberName)
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

        var apiMethod = callerType
            .GetMethods()
            .Where(m => m.Name.Equals(callerMemberName))
            .FirstOrDefault(m => m.HasCustomAttribute<ApiRoute>());

        if (apiMethod is null)
            throw new MissingMethodException(callerType.FullName, callerMemberName);

        var apiRoute = apiMethod.GetCustomAttribute<ApiRoute>();
        var apiRules = apiMethod.GetCustomAttribute<ApiRules>();
        
        if (apiRoute is null)
            throw new MissingAttributeException<ApiRoute>(apiMethod);

        var route = new Route(TwitchApiEndpoint, apiRoute, apiMethod, apiRules);
        
        return route;
    }
    
    private List<ValidationResult> ValidateRoute(Route route, [CallerMemberName] string? callerMemberName = null)
    {
        var results = new List<ValidationResult>();
        var callerMethod = callerMemberName is not null ? GetType().GetMethod(callerMemberName) : null;

        if (callerMethod?.Name.StartsWith(route.ApiRoute.HttpMethod, StringComparison.CurrentCultureIgnoreCase) is false)
        {
            results.Add(new ValidationResult($"Invalid HTTP method for route {route.ApiRoute.Path}"));
        }
        
        foreach (var scope in route.ApiRoute.RequiredScopes)
        {
            if (ClientScopes.Contains(scope) is false)
                results.Add(new ValidationResult($"Missing required scope {scope} on route {route.ApiRoute.Path}"));
        }

        if (Uri.IsWellFormedUriString(route.FullUrl, UriKind.Absolute) is false)
        {
            results.Add(new ValidationResult($"Invalid API route URL: {route.ApiRoute.Path}"));
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
            throw new ApiException($"Route validation failed:\n{string.Join("\n- ", validationResults)}");

        var result = await AsyncHttpClient
            .StartGet(route.FullUrl)
            .AddHeader("Authorization", $"Bearer {ClientAccessToken}")
            .AddHeader("Client-Id", ClientId!)
            .SetQueryString(query)
            .RequireStatusCode(route.ApiRoute.RequiredStatusCode)
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
        
        var route = GetRoute(callerType, callerMemberName);
        
        var validationResults = ValidateRoute(route);
        if (validationResults.Count != 0)
            throw new ApiException($"Route validation failed:\n{string.Join("\n- ", validationResults)}");
        
        var responses = new TFullResponse();
        string? after = null;
        do
        {
            if (query is IWithPagination pagination)
                pagination.After = after;
            
            var result = await AsyncHttpClient
                .StartGet(route.FullUrl)
                .AddHeader("Authorization", $"Bearer {ClientAccessToken}")
                .AddHeader("Client-Id", ClientId!)
                .SetQueryString(query)
                .RequireStatusCode(route.ApiRoute.RequiredStatusCode)
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
        
        var route = GetRoute(callerType, callerMemberName);
        
        var validationResults = ValidateRoute(route);
        if (validationResults.Count != 0)
            throw new ApiException($"Route validation failed:\n{string.Join("\n- ", validationResults)}");

        var result = await AsyncHttpClient
            .StartPost(route.FullUrl)
            .AddHeader("Authorization", $"Bearer {ClientAccessToken}")
            .AddHeader("Client-Id", ClientId!)
            .SetQueryString(query)
            .SetBody(body)
            .RequireStatusCode(route.ApiRoute.RequiredStatusCode)
            .Build()
            .SendAsync<TResponse>(token);

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
            throw new ApiException($"Route validation failed:\n{string.Join("\n- ", validationResults)}");

        var result = await AsyncHttpClient
            .StartPost(route.FullUrl)
            .AddHeader("Authorization", $"Bearer {ClientAccessToken}")
            .AddHeader("Client-Id", ClientId!)
            .SetBody(body)
            .RequireStatusCode(route.ApiRoute.RequiredStatusCode)
            .Build()
            .SendAsync<TResponse>(token);

        return result.Body;
    }
    
    public async Task PostTwitchApiAsync<TQuery>(TQuery? query, Type callerType, CancellationToken token = default, [CallerMemberName] string? callerMemberName = null)
        where TQuery : class, IQueryParameters
    {
        ArgumentException.ThrowIfNullOrEmpty(callerMemberName, nameof(callerMemberName));
        
        var route = GetRoute(callerType, callerMemberName);
        
        var validationResults = ValidateRoute(route);
        if (validationResults.Count != 0)
            throw new ApiException($"Route validation failed:\n{string.Join("\n- ", validationResults)}");

        await AsyncHttpClient
            .StartPost(route.FullUrl)
            .AddHeader("Authorization", $"Bearer {ClientAccessToken}")
            .AddHeader("Client-Id", ClientId!)
            .SetQueryString(query)
            .RequireStatusCode(route.ApiRoute.RequiredStatusCode)
            .Build()
            .SendAsync(token);
    }

    public async Task InjectDataAsync<TTarget>(TTarget target, CancellationToken token = default) where TTarget : class
    {
        var type = typeof(TTarget);
        var properties = type.GetProperties();
        
        foreach (var prop in properties)
        {
            var injectRouteData = prop.GetCustomAttribute<InjectRouteData>();
            
            if (injectRouteData is null)
                continue;

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
}