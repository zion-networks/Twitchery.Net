using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.User;

public class UserUpdateHandler : INotification
{
    public string SubscriptionType => "user.update";
    public string SubscriptionVersion => "1";
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}