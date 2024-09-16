using System.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TwitcheryNet.Attributes;
using TwitcheryNet.Exceptions;
using TwitcheryNet.Http;
using TwitcheryNet.Misc;
using TwitcheryNet.Models.Helix.Channels;
using TwitcheryNet.Models.Helix.Chat;
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

    [ApiRoute("GET", "chat/chatters", "moderator:read:chatters")]
    public async Task<GetChattersResponse?> GetChattersAsync(string broadcasterId, string moderatorId, int? first = null, string? after = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ClientId, nameof(ClientId));
        ArgumentException.ThrowIfNullOrWhiteSpace(AccessToken, nameof(AccessToken));
        MissingTwitchScopeException.ThrowIfMissing(Scopes, "moderator:read:chatters");
        
        var result = await AsyncHttpClient
            .StartGet(TwitchApiEndpoint)
            .SetPath("chat/chatters")
            .AddQueryParameter("broadcaster_id", broadcasterId)
            .AddQueryParameter("moderator_id", moderatorId)
            .AddQueryParameterOptional("first", first?.ToString())
            .AddQueryParameterOptional("after", after)
            .AddHeader("Authorization", $"Bearer {AccessToken}")
            .AddHeader("Client-Id", ClientId)
            .RequireStatusCode(200)
            .Build()
            .SendAsync<GetChattersResponse>(cancellationToken);

        return result;
    }
    
    [ApiRoute("GET", "channels/followers", "moderator:read:followers")]
    public async Task<GetChannelFollowersResponse?> GetChannelFollowersAsync(string broadcasterId, string? userId = null, int? first = null, string? after = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ClientId, nameof(ClientId));
        ArgumentException.ThrowIfNullOrWhiteSpace(AccessToken, nameof(AccessToken));
        MissingTwitchScopeException.ThrowIfMissing(Scopes, "moderator:read:followers");
        
        var result = await AsyncHttpClient
            .StartGet(TwitchApiEndpoint)
            .SetPath("channels/followers")
            .AddQueryParameter("broadcaster_id", broadcasterId)
            .AddQueryParameterOptional("user_id", userId)
            .AddQueryParameterOptional("first", first?.ToString())
            .AddQueryParameterOptional("after", after)
            .AddHeader("Authorization", $"Bearer {AccessToken}")
            .AddHeader("Client-Id", ClientId)
            .RequireStatusCode(200)
            .Build()
            .SendAsync<GetChannelFollowersResponse>(cancellationToken);

        return result;
    }
}