using System.Buffers;
using System.Net.WebSockets;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TwitcheryNet.Events;
using TwitcheryNet.Services.Interfaces;

namespace TwitcheryNet.Net;

public class WebsocketClient : IDisposable
{
    private const int KeepAliveMinTimeoutSeconds = 10;
    private const int KeepAliveMaxTimeoutSeconds = 600;
    private const int MinimumBufferSize = 256;
    private static readonly TimeSpan KeepAliveMinTimeout = TimeSpan.FromSeconds(KeepAliveMinTimeoutSeconds);
    private static readonly TimeSpan KeepAliveMaxTimeout = TimeSpan.FromSeconds(KeepAliveMaxTimeoutSeconds);

    public bool IsConnected => Client.State == WebSocketState.Open;
    public bool IsConnecting => Client.State == WebSocketState.Connecting;
    public bool IsFaulted => Client.CloseStatus != WebSocketCloseStatus.Empty
                             && Client.CloseStatus != WebSocketCloseStatus.NormalClosure;
    
    private string? ReconnectUrl { get; set; }
    
    private ITwitchery Twitch { get; }
    private ILogger<WebsocketClient> Logger { get; }
    private ClientWebSocket Client { get; set; }
    private Task? WebSocketTask { get; set; }
    
    internal event AsyncEventHandler<DataReceivedArgs>? DataReceived;
    internal event AsyncEventHandler<ErrorOccuredArgs>? ErrorOccured; 
    
    public WebsocketClient(ITwitchery twitchery)
    {
        Twitch = twitchery;
        Logger = LoggerFactory
            .Create(builder => builder.AddConsole())
            .CreateLogger<WebsocketClient>();
        
        Client = new ClientWebSocket();
    }
    
    [ActivatorUtilitiesConstructor]
    public WebsocketClient(ILogger<WebsocketClient> logger, ITwitchery twitchery)
    {
        Twitch = twitchery;
        Logger = logger;
        Client = new ClientWebSocket();
    }
    
    public async Task StartAsync(string websocketUri, TimeSpan? keepAliveTimeout = null, CancellationToken token = default)
    {
        keepAliveTimeout ??= TimeSpan.FromSeconds(KeepAliveMaxTimeoutSeconds);
        
        if (keepAliveTimeout.Value < KeepAliveMinTimeout || keepAliveTimeout.Value > KeepAliveMaxTimeout)
        {
            throw new ArgumentException($"Keep alive timeout must be between {KeepAliveMinTimeoutSeconds} " +
                                        $"and {KeepAliveMaxTimeoutSeconds} seconds.", nameof(keepAliveTimeout));
        }
        
        Client.Options.KeepAliveInterval = keepAliveTimeout.Value;

        try
        {
            await Client.ConnectAsync(new Uri(websocketUri), token);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to connect to websocket.");
            ErrorOccured?.Invoke(this, new ErrorOccuredArgs(e));
        }
        
        while (token.IsCancellationRequested is false)
        {
            if (ReconnectUrl is not null)
            {
                await Client.ConnectAsync(new Uri(ReconnectUrl), token);
                ReconnectUrl = null;
            }

            var storeSize = 4096;
            var decoder = Encoding.UTF8.GetDecoder();
            var store = MemoryPool<byte>.Shared.Rent(storeSize).Memory;
            var buffer = MemoryPool<byte>.Shared.Rent(MinimumBufferSize).Memory;
            var payloadSize = 0;

            while (IsConnected)
            {
                ValueWebSocketReceiveResult result = default;
                do
                {
                    try
                    {
                        result = await Client.ReceiveAsync(buffer, token);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e, "Failed to receive message.");
                        ErrorOccured?.Invoke(this, new ErrorOccuredArgs(e));
                        break;
                    }
                    
                    if (payloadSize + result.Count >= storeSize)
                    {
                        storeSize += Math.Max(4096, result.Count);
                        
                        var newStore = MemoryPool<byte>.Shared.Rent(storeSize).Memory;
                        store.CopyTo(newStore);
                        store = newStore;
                    }
                    
                    buffer.CopyTo(store[payloadSize..]);
                    payloadSize += result.Count;
                } while (result.EndOfMessage is false);
                
                switch (result.MessageType)
                {
                    case WebSocketMessageType.Text:
                    {
                        var intermediate = MemoryPool<char>.Shared.Rent(payloadSize).Memory;

                        if (payloadSize == 0)
                            continue;

                        decoder.Convert(store.Span[..payloadSize], intermediate.Span, true, out _, out var charsCount, out _);
                        var message = intermediate[..charsCount];

                        DataReceived?.Invoke(this, new DataReceivedArgs { Message = message.Span.ToString() });
                        payloadSize = 0;
                        
                        break;
                    }
                    case WebSocketMessageType.Binary:
                        Logger.LogWarning("Received binary message.");
                        break;
                    
                    case WebSocketMessageType.Close:
                        Logger.LogWarning("Received close message: {Status} {Description}", Client.CloseStatus, Client.CloseStatusDescription);
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        Logger.LogInformation("Websocket client stopped.");

        if (IsConnected || IsConnecting)
            await Client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Stopped", token);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        
        Client.Dispose();
        WebSocketTask?.Dispose();
    }
}