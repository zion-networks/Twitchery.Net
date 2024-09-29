using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.SharedChat;

public class ChannelSharedChatSessionBeginHandler : INotification
{
    public string SubscriptionType => "channel.shared_chat.begin";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelSharedChatSessionBeginHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelSharedChatSessionBeginHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        return Task.CompletedTask;
    }
}