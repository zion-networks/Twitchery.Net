using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.User.Authorization;

public class UserAuthorizationGrantHandler : INotification
{
    public string SubscriptionType => "user.authorization.grant";
    public string SubscriptionVersion => "1";
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}