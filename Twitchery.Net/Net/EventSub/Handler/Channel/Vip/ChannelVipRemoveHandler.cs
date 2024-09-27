using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.Vip;

public class ChannelVipRemoveHandler : INotification
{
    public string SubscriptionType => "channel.vip.remove";
    public string SubscriptionVersion => "1";
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}