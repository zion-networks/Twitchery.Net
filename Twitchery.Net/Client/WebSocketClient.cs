using System.Net.WebSockets;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TwitcheryNet.Models.Client;
using TwitcheryNet.Services.Interfaces;

namespace TwitcheryNet.Client;

public class WebSocketClient
{
    private const string TwitchWebSocketUrl = "wss://eventsub.wss.twitch.tv/ws";
    private const int KeepAliveMinTimeoutSeconds = 10;
    private const int KeepAliveMaxTimeoutSeconds = 600;
    private static readonly TimeSpan KeepAliveMinTimeout = TimeSpan.FromSeconds(KeepAliveMinTimeoutSeconds);
    private static readonly TimeSpan KeepAliveMaxTimeout = TimeSpan.FromSeconds(KeepAliveMaxTimeoutSeconds);
    
    private string? ReconnectUrl { get; set; }
    
    private ITwitchery Twitch { get; }
    private ILogger<WebSocketClient> Logger { get; }
    private ClientWebSocket? Client { get; set; }
    
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
        
        return Task.CompletedTask;
    }
}