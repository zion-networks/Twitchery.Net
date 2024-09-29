using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.Warning;

public class ChannelWarningAcknowledgementHandler : INotification
{
    public string SubscriptionType => "channel.warning.acknowledge";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelWarningAcknowledgementHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelWarningAcknowledgementHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}