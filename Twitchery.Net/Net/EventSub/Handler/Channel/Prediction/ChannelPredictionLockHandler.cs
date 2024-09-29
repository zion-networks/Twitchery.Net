using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.Prediction;

public class ChannelPredictionLockHandler : INotification
{
    public string SubscriptionType => "channel.prediction.lock";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelPredictionLockHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelPredictionLockHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}