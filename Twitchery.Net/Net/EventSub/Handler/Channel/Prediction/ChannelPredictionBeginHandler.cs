using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.Prediction;

public class ChannelPredictionBeginHandler : INotification
{
    public string SubscriptionType => "channel.prediction.begin";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelPredictionBeginHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelPredictionBeginHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}