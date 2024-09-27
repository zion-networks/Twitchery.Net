using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TwitcheryNet.Attributes;
using TwitcheryNet.Events;
using TwitcheryNet.Exceptions;
using TwitcheryNet.Extensions;
using TwitcheryNet.Misc;
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
    private Dictionary<string, Func<EventSubClient, string, Task>> Handlers { get; } = new();
    private Dictionary<string, Delegate> Listener { get; } = new();
    
    private string SessionId { get; set; } = string.Empty;
    
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

        InitializerHandlers();
        
        Client.DataReceived += OnDataReceived;
        Client.ErrorOccured += OnErrorOccured; 
    }
    
    [ActivatorUtilitiesConstructor]
    public EventSubClient(ILogger<EventSubClient> logger, ITwitchery twitchery)
    {
        Logger = logger;
        Twitch = twitchery;
        Client = new WebsocketClient(twitchery);

        InitializerHandlers();
        
        Client.DataReceived += OnDataReceived;
        Client.ErrorOccured += OnErrorOccured;
    }

    private void InitializerHandlers()
    {
        var handlers = typeof(INotification)
            .Assembly
            .ExportedTypes
            .Where(x => typeof(INotification).IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false })
            .Select(Activator.CreateInstance)
            .Cast<INotification>()
            .ToList();
        
        Handlers.Clear();
        
        foreach (var handler in handlers)
        {
            Logger.LogDebug("Registering EventSub handler for {SubscriptionType}", handler.SubscriptionType);
            Handlers.Add(handler.SubscriptionType, handler.Handle);
        }
    }

    public Task StartAsync(CancellationToken token = default)
    {
        _lastReceived = DateTimeOffset.MinValue;
        
#pragma warning disable CS4014
        Client.StartAsync(TwitchWebSocketUrl, token: token);
#pragma warning restore CS4014
        
        return Task.CompletedTask;
    }

    private Task OnErrorOccured(object sender, ErrorOccuredArgs e)
    {
        Logger.LogError(e.Exception, "An error occured while receiving data.");
        
        return Task.CompletedTask;
    }

    private async Task OnDataReceived(object sender, DataReceivedArgs args)
    {
        _lastReceived = DateTimeOffset.Now;

        WebSocketMessage? msg;
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

        SessionWelcomeMessage? json;

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

    private async Task HandleNotificationAsync(string msg)
    {
        var msgJson = JsonConvert.DeserializeObject<WebSocketMessage>(msg);
        
        if (msgJson is null)
        {
            Logger.LogError("Failed to deserialize WebSocketMessage");
            return;
        }

        var subType = msgJson.Metadata.SubscriptionType;
        
        if (Handlers.TryGetValue(subType, out var handler))
        {
            await handler.Invoke(this, msg);
        }
        else
        {
            Logger.LogWarning("No handler found for {SubscriptionType}", subType);
        }
    }

    private async Task HandleRevocationAsync(string msg)
    {
        Logger.LogDebug("Received Revocation!");

        var data = JsonConvert.DeserializeObject<EventSubNotificationData>(msg);
        
        if (data is null)
        {
            Logger.LogError("Failed to deserialize EventSubNotificationData<NotificationSubscription>");
            return;
        }

        var subId = data.Payload.Subscription.Id;
        var subType = data.Payload.Subscription.Type;
        var currentListener = Listener.GetValueOrDefault(subId);

        if (currentListener is null)
        {
            Logger.LogWarning("No listener found for {SubscriptionType} with Id {SubscriptionId}", subType, subId);
            return;
        }

        if (currentListener.Target is null)
        {
            Logger.LogWarning("Listener target is null for {SubscriptionType} with Id {SubscriptionId}", subType, subId);
            return;
        }
        
        await SubscribeAsync(currentListener.Target, data.Metadata.SubscriptionType, data.Metadata.SubscriptionVersion, currentListener);
        
        Logger.LogInformation("Re-registered listener for {SubscriptionType} with Id {SubscriptionId}", subType, subId);
        
        Listener.Remove(subId);
    }
    
    public async Task RegisterEventSubAsync(object source, string eventName, Delegate handler)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentException.ThrowIfNullOrEmpty(eventName, nameof(eventName));
        ArgumentNullException.ThrowIfNull(handler, nameof(handler));

        var tSource = source.GetType();
        var tEvent = tSource.GetEvent(eventName);
        
        if (tEvent is null)
        {
            Logger.LogError("Event {EventName} not found in {SourceType}", eventName, tSource);
            return;
        }
        
        if (tEvent.TryGetCustomAttribute<EventSubAttribute>(out var eventSub) is false || eventSub is null)
        {
            return;
        }
        
        MissingTwitchScopeException.ThrowIfMissing(Twitch.ClientScopes, eventSub.RequiredScopes);
        
        var eventType = eventSub.EventSubType;
        var eventVersion = eventSub.EventSubVersion;
        
        await SubscribeAsync(source, eventType, eventVersion, handler);
    }

    public async Task UnregisterEventSubAsync(object source, string eventName, Delegate handler)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentException.ThrowIfNullOrEmpty(eventName, nameof(eventName));
        ArgumentNullException.ThrowIfNull(handler, nameof(handler));
        
        var tSource = source.GetType();
        var tEvent = tSource.GetEvent(eventName);
        
        if (tEvent is null)
        {
            Logger.LogError("Event {EventName} not found in {SourceType}", eventName, tSource);
            return;
        }
        
        if (tEvent.TryGetCustomAttribute<EventSubAttribute>(out var eventSub) is false || eventSub is null)
        {
            return;
        }
        
        MissingTwitchScopeException.ThrowIfMissing(Twitch.ClientScopes, eventSub.RequiredScopes);
        
        var eventType = eventSub.EventSubType;
        var eventVersion = eventSub.EventSubVersion;
        
        // TODO: required DELETE http method to be implemented in AsyncHttpClient and Twitchery
    }
    
    [ApiRoute("POST", "eventsub/subscriptions", "channel:read:subscriptions", RequiredStatusCode = HttpStatusCode.Accepted)]
    public async Task SubscribeAsync(object source, string eventType, string eventVersion, Delegate eventHandler)
    {
        var sourceType = source.GetType();
        
        if (typeof(IConditional).IsAssignableFrom(sourceType) is false)
        {
            Logger.LogError("Source object does not implement IConditional");
            return;
        }
        
        if (sourceType.IsInterface || sourceType.IsAbstract)
        {
            Logger.LogError("Source object cannot be an interface or abstract class");
            return;
        }

        if (source is not IConditional conditionalSource)
        {
            Logger.LogError("Failed to cast source object to IConditional");
            return;
        }
        
        if (Client.IsConnected is false)
        {
            await StartAsync();
        }
        
        ArgumentException.ThrowIfNullOrWhiteSpace(eventType);
        ArgumentException.ThrowIfNullOrWhiteSpace(eventVersion);

        var request = new CreateEventSubSubscriptionRequestBody(eventType, eventVersion)
        {
            Condition = conditionalSource.ToCondition(),
            Transport =
            {
                SessionId = await WaitForSessionIdAsync(TaskUtils.TimeoutToken())
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
        
        Listener.Add(resData.Id, eventHandler);
        
        Logger.LogInformation("Registered for event {Key}[v{Version}]: {Status}", eventType, eventVersion, response.Data[0].Status);
    }

    [ApiRoute("POST", "eventsub/subscriptions", "channel:read:subscriptions", RequiredStatusCode = HttpStatusCode.Accepted)]
    public async Task SubscribeAsync<T>(T source, string eventType, string eventVersion, Delegate eventHandler) where T : class, IConditional
    {
        await SubscribeAsync((object)source, eventType, eventVersion, eventHandler);
    }

    private async Task<string> WaitForSessionIdAsync(CancellationToken token = default)
    {
        while (string.IsNullOrWhiteSpace(SessionId))
        {
            await Task.Delay(10, token);
        }
                
        return SessionId;
    }

    public async Task RaiseEventAsync<T>(string subscriptionType, EventSubNotificationData<T> data)
        where T : class, new()
    {
        var subId = data.Payload.Subscription.Id;
        
        if (Listener.TryGetValue(subId, out var listener))
        {
            try
            {
                data.Payload.Event.InjectTwitchery(Twitch);
                
                if (listener.Method.ReturnType == typeof(Task))
                {
                    Logger.LogDebug("Invoking async listener delegate method {Delegate} for {SubscriptionType}", listener.Method, subscriptionType);
                    if (listener.Method.Invoke(listener.Target, [ listener.Target, data.Payload.Event ]) is Task awaitableListenerHandler)
                    {
                        await awaitableListenerHandler;
                    }
                    else
                    {
                        Logger.LogError("Cannot invoke async listener, because the return value is " +
                                        "expected to of type Task for Delegate method {Delegate} for " +
                                        "{SubscriptionType}", listener.Method, subscriptionType);
                    }
                }
                else
                {
                    Logger.LogDebug("Invoking listener delegate method {Delegate} for {SubscriptionType}", listener.Method, subscriptionType);
                    listener.Method.Invoke(listener.Target, [ listener.Target, data.Payload.Event ]);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to invoke listener delegate method {Delegate} for {SubscriptionType}", listener.Method, subscriptionType);
            }
        }
    }
}