using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Drop.Entitlement;

public class DropEntitlementGrantHandler : INotification
{
    public string SubscriptionType => "drop.entitlement.grant";
    public string SubscriptionVersion => "1";
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}