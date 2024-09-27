using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.Vip;

public class ChannelVipAddHandler : INotification
{
    public string SubscriptionType => "channel.vip.add";
    public string SubscriptionVersion => "1";
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}