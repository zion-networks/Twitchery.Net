using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.ChannelPointsCustomReward;

public class ChannelPointsCustomRewardRemoveHandler : INotification
{
    public string SubscriptionType => "channel.channel_points_custom_reward.remove";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelPointsCustomRewardRemoveHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelPointsCustomRewardRemoveHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}