using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.User.Authorization;

public class UserAuthorizationRevokeHandler : INotification
{
    public string SubscriptionType => "user.authorization.revoke";
    public string SubscriptionVersion => "1";
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}