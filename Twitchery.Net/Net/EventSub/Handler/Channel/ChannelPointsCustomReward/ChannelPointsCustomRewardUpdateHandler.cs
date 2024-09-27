using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.ChannelPointsCustomReward;

public class ChannelPointsCustomRewardUpdateHandler : INotification
{
    public string SubscriptionType => "channel.channel_points_custom_reward.update";
    public string SubscriptionVersion => "1";
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}