using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TwitcheryNet.Attributes;
using TwitcheryNet.Events;
using TwitcheryNet.Extensions;
using TwitcheryNet.Models.Client;
using TwitcheryNet.Models.Client.Messages.Welcome;
using TwitcheryNet.Models.Helix.EventSub.Subscriptions;
using TwitcheryNet.Net.EventSub.EventArgs;
using TwitcheryNet.Services.Interfaces;

namespace TwitcheryNet.Net.EventSub;

public class EventSubClient
{
    private ILogger<EventSubClient> Logger { get; }
    private ITwitchery Twitch { get; }
    private WebsocketClient Client { get; }
    private Dictionary<string, Action<EventSubNotification>> Handlers { get; } = new();
    
    private string? SessionId { get; set; }
    
    private const string TwitchWebSocketUrl = "wss://eventsub.wss.twitch.tv/ws";
    
    private DateTimeOffset _lastReceived;

    public EventSubClient(ITwitchery twitchery)
    {
        Logger = LoggerFactory
            .Create(builder =>
            {
                builder.AddConsole().SetMinimumLevel(LogLevel.Debug);
            })
            .CreateLogger<EventSubClient>();
        
        Twitch = twitchery;
        Client = new WebsocketClient(twitchery);

        Client.DataReceived += OnDataReceived;
        Client.ErrorOccured += OnErrorOccured;
    }
    
    [ActivatorUtilitiesConstructor]
    public EventSubClient(ILogger<EventSubClient> logger, ITwitchery twitchery)
    {
        Logger = logger;
        Twitch = twitchery;
        Client = new WebsocketClient(twitchery);

        Client.DataReceived += OnDataReceived;
        Client.ErrorOccured += OnErrorOccured;
    }

    public async Task StartAsync(CancellationToken token = default)
    {
        _lastReceived = DateTimeOffset.MinValue;
        
        Client.StartAsync(TwitchWebSocketUrl, token: token);
    }

    private Task OnErrorOccured(object sender, ErrorOccuredArgs e)
    {
        Logger.LogError(e.Exception, "An error occured while receiving data.");
        
        return Task.CompletedTask;
    }

    private async Task OnDataReceived(object sender, DataReceivedArgs args)
    {
        _lastReceived = DateTimeOffset.Now;

        WebSocketMessage? msg = null;
        try
        {
            msg = JsonConvert.DeserializeObject<WebSocketMessage>(args.Message);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to parse JSON.");
            return;
        }

        if (msg is null)
        {
            Logger.LogError("Failed to parse WebSocketMessage!");
            return;
        }

        switch (msg.Metadata.Type)
        {
            case "session_welcome":
                await HandleSessionWelcomeAsync(args.Message);
                break;
            
            case "session_disconnect":
                await HandleSessionDisconnectAsync(args.Message);
                break;
            
            case "session_reconnect":
                await HandleSessionReconnectAsync(args.Message);
                break;
            
            case "session_keepalive":
                await HandleSessionKeepaliveAsync(args.Message);
                break;
            
            case "notification":
                await HandleNotificationAsync(args.Message);
                break;
            
            case "revocation":
                await HandleRevocationAsync(args.Message);
                break;
            
            default:
                Logger.LogError("Received invalid WebSocketMessage type: {Type}", msg.Metadata.Type);
                break;
        }
    }

    private Task HandleSessionWelcomeAsync(string msg)
    {
        ArgumentNullException.ThrowIfNull(msg);

        SessionWelcomeMessage? json = null;

        try
        {
            json = JsonConvert.DeserializeObject<SessionWelcomeMessage>(msg);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to deserialize SessionWelcomeMessage");
            return Task.CompletedTask;
        }

        if (json is null)
        {
            Logger.LogError("Failed to deserialize SessionWelcomeMessage");
            return Task.CompletedTask;
        }

        SessionId = json.Payload.Session.Id;

        if (string.IsNullOrWhiteSpace(SessionId))
        {
            Logger.LogError("Received empty SessionId in SessionWelcomeMessage");
            return Task.CompletedTask;
        }

        Logger.LogDebug("Received SessionId from SessionWelcomeMessage");

        return Task.CompletedTask;
    }

    private Task HandleSessionDisconnectAsync(string msg)
    {
        Logger.LogDebug("Received SessionDisconnect!");
        return Task.CompletedTask;
    }

    private Task HandleSessionReconnectAsync(string msg)
    {
        Logger.LogDebug("Received SessionReconnect!");
        return Task.CompletedTask;
    }

    private Task HandleSessionKeepaliveAsync(string msg)
    {
        Logger.LogDebug("Received SessionKeepalive!");
        return Task.CompletedTask;
    }

    private Task HandleNotificationAsync(string msg)
    {
        //var baseMsg = JsonConvert.DeserializeObject<EventSubNotificationData<>>(msg);

        //if (baseMsg is not null)
        //{
        //    Logger.LogInformation("Received Notification for {SubType}", baseMsg.Metadata.SubscriptionType);
        //}
        //else
        //{
        //    Logger.LogWarning("Failed to deserialize Notification");
        //}
        //
        //var txt = JsonConvert.SerializeObject(baseMsg?.Metadata);
        //Logger.LogDebug("Notification Metadata: {Metadata}", txt);
        
        return Task.CompletedTask;
    }

    private Task HandleRevocationAsync(string msg)
    {
        Logger.LogDebug("Received Revocation!");
        return Task.CompletedTask;
    }

    [ApiRoute("POST", "eventsub/subscriptions", "channel:read:subscriptions", RequiredStatusCode = HttpStatusCode.Accepted)]
    public async Task SubscribeAsync<T>(T source, Enum eventKey, Action<EventSubNotification> handler)
        where T : class, IConditional
    {
        if (Client.IsConnected is false)
            await StartAsync();
        
        var eventType = eventKey.GetValue();
        var eventVersion = eventKey.GetVersion();
        
        ArgumentException.ThrowIfNullOrWhiteSpace(eventType);
        ArgumentException.ThrowIfNullOrWhiteSpace(eventVersion);

        var request = new CreateEventSubSubscriptionRequestBody(eventType, eventVersion)
        {
            Condition = source.ToCondition(),
            Transport =
            {
                SessionId = SessionId
            }
        };

        var response = await Twitch.PostTwitchApiAsync<CreateEventSubSubscriptionRequestBody, CreateEventSubSubscriptionResponseBody>(
            request, typeof(EventSubClient));
        
        if (response is null || response.Data.Count == 0)
        {
            Logger.LogError("Failed to subscribe to {EventKey}[v{Version}]", eventType, eventVersion);
            return;
        }
        
        var resData = response.Data[0];

        Handlers.TryAdd(resData.Id, handler);
        
        Logger.LogInformation("Registered for event {Key}[v{Version}]: {Status}", eventType, eventVersion, response.Data[0].Status);
    }
}