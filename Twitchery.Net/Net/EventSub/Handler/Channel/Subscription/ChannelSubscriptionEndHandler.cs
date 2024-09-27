using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.Subscription;

public class ChannelSubscriptionEndHandler : INotification
{
    public string SubscriptionType => "channel.subscription.end";
    public string SubscriptionVersion => "1";
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        return Task.CompletedTask;
    }
}