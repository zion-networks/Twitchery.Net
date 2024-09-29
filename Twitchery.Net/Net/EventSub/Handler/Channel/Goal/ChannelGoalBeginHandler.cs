using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.Goal;

public class ChannelGoalBeginHandler : INotification
{
    public string SubscriptionType => "channel.goal.begin";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelGoalBeginHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelGoalBeginHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}