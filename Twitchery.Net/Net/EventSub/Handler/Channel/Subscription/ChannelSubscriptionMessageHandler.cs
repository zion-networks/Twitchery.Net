using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.Subscription;

public class ChannelSubscriptionMessageHandler : INotification
{
    public string SubscriptionType => "channel.subscription.message";
    public string SubscriptionVersion => "1";
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        return Task.CompletedTask;
    }
}