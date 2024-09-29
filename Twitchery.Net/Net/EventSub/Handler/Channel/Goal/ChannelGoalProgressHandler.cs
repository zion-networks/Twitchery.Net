using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.Goal;

public class ChannelGoalProgressHandler : INotification
{
    public string SubscriptionType => "channel.goal.progress";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelGoalProgressHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelGoalProgressHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}