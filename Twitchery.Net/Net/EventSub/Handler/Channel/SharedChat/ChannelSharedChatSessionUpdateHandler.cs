using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.SharedChat;

public class ChannelSharedChatSessionUpdateHandler : INotification
{
    public string SubscriptionType => "channel.shared_chat.update";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelSharedChatSessionUpdateHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelSharedChatSessionUpdateHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        return Task.CompletedTask;
    }
}