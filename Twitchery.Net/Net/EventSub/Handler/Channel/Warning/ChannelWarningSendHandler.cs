using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.Warning;

public class ChannelWarningSendHandler : INotification
{
    public string SubscriptionType => "channel.warning.send";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelWarningSendHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelWarningSendHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}