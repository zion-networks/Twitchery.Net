using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.AdBreak;

public class ChannelAdBreakBeginHandler : INotification
{
    public string SubscriptionType => "channel.ad_break.begin";
    public string SubscriptionVersion => "1";
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}