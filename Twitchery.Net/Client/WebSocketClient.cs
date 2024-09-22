using System.Net;
using System.Net.WebSockets;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TwitcheryNet.Attributes;
using TwitcheryNet.Client.EventArgs;
using TwitcheryNet.Extensions;
using TwitcheryNet.Models.Client;
using TwitcheryNet.Models.Helix.Channels;
using TwitcheryNet.Models.Helix.EventSub.Subscriptions;
using TwitcheryNet.Services.Interfaces;

namespace TwitcheryNet.Client;

public class WebSocketClient
{
    private const string TwitchWebSocketUrl = "wss://eventsub.wss.twitch.tv/ws";
    private const int KeepAliveMinTimeoutSeconds = 10;
    private const int KeepAliveMaxTimeoutSeconds = 600;
    private static readonly TimeSpan KeepAliveMinTimeout = TimeSpan.FromSeconds(KeepAliveMinTimeoutSeconds);
    private static readonly TimeSpan KeepAliveMaxTimeout = TimeSpan.FromSeconds(KeepAliveMaxTimeoutSeconds);
    
    private string? SessionId { get; set; }
    private string? ReconnectUrl { get; set; }
    
    private ITwitchery Twitch { get; }
    private ILogger<WebSocketClient> Logger { get; }
    private ClientWebSocket? Client { get; set; }
    private Task? WebSocketTask { get; set; }
    
    public WebSocketClient(ITwitchery twitchery)
    {
        Twitch = twitchery;
        Logger = LoggerFactory
            .Create(builder => builder.AddConsole())
            .CreateLogger<WebSocketClient>();
    }
    
    [ActivatorUtilitiesConstructor]
    public WebSocketClient(ILogger<WebSocketClient> logger, ITwitchery twitchery)
    {
        Twitch = twitchery;
        Logger = logger;
    }
    
    public async Task StartAsync(TimeSpan? keepAliveTimeout = null, CancellationToken token = default)
    {
        keepAliveTimeout ??= TimeSpan.FromSeconds(30);
        
        if (keepAliveTimeout.Value < KeepAliveMinTimeout || keepAliveTimeout.Value > KeepAliveMaxTimeout)
        {
            throw new ArgumentException($"Keep alive timeout must be between {KeepAliveMinTimeoutSeconds} " +
                                        $"and {KeepAliveMaxTimeoutSeconds} seconds.", nameof(keepAliveTimeout));
        }
        
        Client = new ClientWebSocket();
        Client.Options.KeepAliveInterval = keepAliveTimeout.Value;
        
        await Client.ConnectAsync(new Uri(TwitchWebSocketUrl), token);
        while (token.IsCancellationRequested is false)
        {
            if (ReconnectUrl is not null)
            {
                await Client.ConnectAsync(new Uri(ReconnectUrl), token);
                ReconnectUrl = null;
            }
            
            var buffer = new ArraySegment<byte>(new byte[1024]);
            var result = await Client.ReceiveAsync(buffer, token);
            
            if (result.MessageType is WebSocketMessageType.Close)
            {
                Logger.LogInformation("Closing connection: {Status} {Description}", result.CloseStatus, result.CloseStatusDescription);
                await Client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", token);
                
                WebSocketTask = null;
                SessionId = null;
                
                break;
            }
            
            var message = Encoding.UTF8.GetString(buffer.Array!, 0, result.Count);
            
            await HandleMessageAsync(message);
        }
    }
    
    private async Task HandleMessageAsync(string message)
    {
        var msg = JsonConvert.DeserializeObject<WebSocketMessage>(message) ?? throw new InvalidOperationException("Failed to deserialize message.");
        var type = msg.Metadata.Type;

        switch (type)
        {
            case "session_welcome":
                await HandleSessionWelcomeAsync(msg);
                break;
            
            case "session_keepalive":
                await HandleSessionKeepAliveAsync(msg);
                break;
            
            case "session_reconnect":
                await HandleSessionReconnectAsync(msg);
                break;
            
            default:
                Logger.LogWarning("Unhandled message type: {Type}", type);
                break;
        }
    }

    private async Task HandleSessionReconnectAsync(WebSocketMessage msg)
    {
        ReconnectUrl = msg.Payload.Session.ReconnectUrl;
        Logger.LogInformation("Session reconnect: {ReconnectUrl}", ReconnectUrl);
        
        if (Client is not null)
            await Client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Reconnecting", CancellationToken.None);
    }

    private Task HandleSessionKeepAliveAsync(WebSocketMessage msg)
    {
        Logger.LogInformation("Session keep alive: {Id} {Timestamp}", msg.Metadata.Id, msg.Metadata.Timestamp);
        return Task.CompletedTask;
    }

    private Task HandleSessionWelcomeAsync(WebSocketMessage message)
    {
        var session = message.Payload.Session;
        var connectedAt = session.ConnectedAt;
        var keepAliveTimeout = session.KeepAliveTimeoutSeconds;
        var reconnectUrl = session.ReconnectUrl;
        
        Logger.LogInformation("Session welcome: {ConnectedAt} {KeepAliveTimeout} {ReconnectUrl}",
            connectedAt, keepAliveTimeout, reconnectUrl);
        
        SessionId = session.Id;
        
        return Task.CompletedTask;
    }

    [ApiRoute("POST", "eventsub/subscriptions", "channel:read:subscriptions", RequiredStatusCode = HttpStatusCode.Accepted)]
    public async Task SubscribeEventAsync<TSource, TArgs>(TSource source, Enum eventKey, EventHandler<TArgs> handler)
        where TArgs : EventBaseEventArgs
        where TSource : class
    {
        var keyValue = eventKey.GetValue();
        ArgumentException.ThrowIfNullOrWhiteSpace(keyValue);

        if (source is Channel channel)
        {
            await SubscribeChannelEventAsync(channel, (Events.Channel)eventKey, handler);
        }
    }

    private async Task SubscribeChannelEventAsync<TArgs>(Channel channel, Events.Channel eventKey, EventHandler<TArgs> handler)
        where TArgs : EventBaseEventArgs
    {
        var eventKeyValue = eventKey.GetValue();
        
        ArgumentException.ThrowIfNullOrWhiteSpace(eventKeyValue, nameof(eventKey));

        switch (eventKey)
        {
            case Events.Channel.NewFollower:
                await SubscribeNewFollowerEventAsync(channel, eventKeyValue);
                break;
            
            case Events.Channel.ChatMessage:
                await SubscribeChatMessageEventAsync(channel, eventKeyValue);
                break;
            
            default:
                Logger.LogWarning("Unhandled event key: {EventKey}", eventKey);
                break;
        }
    }

    private async Task SubscribeChatMessageEventAsync(Channel channel, string eventKeyValue)
    {
        var userId = Twitch.Me?.Id ?? throw new InvalidOperationException("Missing broadcaster user id.");
        var body = new CreateEventSubSubscriptionRequestBody(eventKeyValue, "1")
        {
            Condition = new EventSubConditions
            {
                BroadcasterUserId = channel.BroadcasterId,
                UserId = userId
            }
        };
        
        if (WebSocketTask is null)
        {
            WebSocketTask = StartAsync();
        }

        body.Transport.SessionId = await Task.Run(async () =>
        {
            while (SessionId is null)
            {
                await Task.Delay(100);
            }

            return SessionId;
        }, new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token) ?? throw new InvalidOperationException("Failed to get session id.");
        
        var response = await Twitch.PostTwitchApiAsync<CreateEventSubSubscriptionRequestBody, CreateEventSubSubscriptionResponseBody>(body, typeof(WebSocketClient), default, nameof(SubscribeEventAsync));

        if (response is null)
        {
            Logger.LogError("Failed to subscribe to {EventKey} event.", eventKeyValue);
        }
        else
        {
            Logger.LogInformation("Subscribed to {EventKey} event at {ConnectedAt}", eventKeyValue, response.ConnectedAt);
        }
    }

    private async Task SubscribeNewFollowerEventAsync(Channel channel, string eventKeyValue)
    {
        var moderatorId = Twitch.Me?.Id ?? throw new InvalidOperationException("Missing broadcaster user id.");
        var body = new CreateEventSubSubscriptionRequestBody(eventKeyValue, "2")
        {
            Condition = new EventSubConditions
            {
                BroadcasterUserId = channel.BroadcasterId,
                ModeratorUserId = moderatorId
            }
        };
        
        if (WebSocketTask is null)
        {
            WebSocketTask = StartAsync();
        }

        body.Transport.SessionId = await Task.Run(async () =>
        {
            while (SessionId is null)
            {
                await Task.Delay(100);
            }

            return SessionId;
        }, new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token) ?? throw new InvalidOperationException("Failed to get session id.");
        
        var response = await Twitch.PostTwitchApiAsync<CreateEventSubSubscriptionRequestBody, CreateEventSubSubscriptionResponseBody>(body, typeof(WebSocketClient), default, nameof(SubscribeEventAsync));

        if (response is null)
        {
            Logger.LogError("Failed to subscribe to {EventKey} event.", eventKeyValue);
        }
        else
        {
            Logger.LogInformation("Subscribed to {EventKey} event at {ConnectedAt}", eventKeyValue, response.ConnectedAt);
        }
    }
}