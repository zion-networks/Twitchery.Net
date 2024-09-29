using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.SuspiciousUser;

public class ChannelSuspiciousUserUpdateHandler : INotification
{
    public string SubscriptionType => "channel.suspicious_user.update";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelSuspiciousUserUpdateHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelSuspiciousUserUpdateHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}