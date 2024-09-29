using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.SuspiciousUser;

public class ChannelSuspiciousUserMessageHandler : INotification
{
    public string SubscriptionType => "channel.suspicious_user.message";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelSuspiciousUserMessageHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelSuspiciousUserMessageHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}