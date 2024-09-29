using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.ShieldMode;

public class ChannelShieldModeEndHandler : INotification
{
    public string SubscriptionType => "channel.shield_mode.end";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelShieldModeEndHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelShieldModeEndHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}