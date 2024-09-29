using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.ChannelPointsCustomReward;

public class ChannelPointsCustomRewardAddHandler : INotification
{
    public string SubscriptionType => "channel.channel_points_custom_reward.add";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelPointsCustomRewardAddHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelPointsCustomRewardAddHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}