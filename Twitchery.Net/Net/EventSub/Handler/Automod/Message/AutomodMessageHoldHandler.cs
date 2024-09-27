using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Automod.Message;

public class AutomodMessageHoldHandler : INotification
{
    public string SubscriptionType => "automod.message.hold";
    public string SubscriptionVersion => "1";

    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}