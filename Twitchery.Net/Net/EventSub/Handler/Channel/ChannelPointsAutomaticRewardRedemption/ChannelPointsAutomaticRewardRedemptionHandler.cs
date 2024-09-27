using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.ChannelPointsAutomaticRewardRedemption;

public class ChannelPointsAutomaticRewardRedemptionHandler : INotification
{
    public string SubscriptionType => "channel.channel_points_automatic_reward_redemption.add";
    public string SubscriptionVersion => "1";
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}