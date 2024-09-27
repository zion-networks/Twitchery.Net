using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel;

public class ChannelUpdateHandler : INotification
{
    public string SubscriptionType => "channel.update";
    public string SubscriptionVersion => "2";
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        return Task.CompletedTask;
    }
}