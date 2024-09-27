using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel;

public class ChannelModerateHandler : INotification
{
    public string SubscriptionType => "channel.moderate";
    public string SubscriptionVersion => "2"; // v1 will not be implemented
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        return Task.CompletedTask;
    }
}