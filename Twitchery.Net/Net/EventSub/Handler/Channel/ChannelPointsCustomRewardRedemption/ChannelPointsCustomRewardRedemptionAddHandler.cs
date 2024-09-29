using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.ChannelPointsCustomRewardRedemption;

public class ChannelPointsCustomRewardRedemptionAddHandler : INotification
{
    public string SubscriptionType => "channel.channel_points_custom_reward_redemption.add";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelPointsCustomRewardRedemptionAddHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelPointsCustomRewardRedemptionAddHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}