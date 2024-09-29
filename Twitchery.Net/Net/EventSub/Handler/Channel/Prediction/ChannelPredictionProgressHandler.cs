using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.Prediction;

public class ChannelPredictionProgressHandler : INotification
{
    public string SubscriptionType => "channel.prediction.progress";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelPredictionProgressHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelPredictionProgressHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}