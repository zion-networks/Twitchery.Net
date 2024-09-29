using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.HypeTrain;

public class ChannelHypeTrainEndHandler : INotification
{
    public string SubscriptionType => "channel.hype_train.end";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelHypeTrainEndHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelHypeTrainEndHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}