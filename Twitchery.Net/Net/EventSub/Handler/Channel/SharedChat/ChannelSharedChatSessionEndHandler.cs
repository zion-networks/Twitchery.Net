using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.SharedChat;

public class ChannelSharedChatSessionEndHandler : INotification
{
    public string SubscriptionType => "channel.shared_chat.end";
    public string SubscriptionVersion => "1";
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        return Task.CompletedTask;
    }
}