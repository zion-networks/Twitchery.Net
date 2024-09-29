using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.User.Authorization;

public class UserAuthorizationRevokeHandler : INotification
{
    public string SubscriptionType => "user.authorization.revoke";
    public string SubscriptionVersion => "1";
    
    private ILogger<UserAuthorizationRevokeHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<UserAuthorizationRevokeHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}