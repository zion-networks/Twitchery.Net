using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.User.Whisper;

public class UserWhisperMessageHandler : INotification
{
    public string SubscriptionType => "user.whisper.message";
    public string SubscriptionVersion => "1";
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}