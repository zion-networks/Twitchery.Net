using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.Prediction;

public class ChannelPredictionEndHandler : INotification
{
    public string SubscriptionType => "channel.prediction.end";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelPredictionEndHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelPredictionEndHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}