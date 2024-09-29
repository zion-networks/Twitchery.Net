using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.ChannelPointsCustomRewardRedemption;

public class ChannelPointsCustomRewardRedemptionUpdateHandler : INotification
{
    public string SubscriptionType => "channel.channel_points_custom_reward_redemption.update";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelPointsCustomRewardRedemptionUpdateHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelPointsCustomRewardRedemptionUpdateHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}