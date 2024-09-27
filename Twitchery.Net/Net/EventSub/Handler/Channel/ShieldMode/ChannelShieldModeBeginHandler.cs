using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.ShieldMode;

public class ChannelShieldModeBeginHandler : INotification
{
    public string SubscriptionType => "channel.shield_mode.begin";
    public string SubscriptionVersion => "1";
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}