using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel;

public class ChannelFollowHandler : INotification
{
    public string SubscriptionType => "channel.follow";
    public string SubscriptionVersion => "2";
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        return Task.CompletedTask;
    }
}