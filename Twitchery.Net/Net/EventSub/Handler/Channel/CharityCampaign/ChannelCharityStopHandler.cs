using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.CharityCampaign;

public class ChannelCharityStopHandler : INotification
{
    public string SubscriptionType => "channel.charity_campaign.stop";
    public string SubscriptionVersion => "1";
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}