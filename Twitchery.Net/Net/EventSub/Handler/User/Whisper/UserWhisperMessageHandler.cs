using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.User.Whisper;

public class UserWhisperMessageHandler : INotification
{
    public string SubscriptionType => "user.whisper.message";
    public string SubscriptionVersion => "1";
    
    private ILogger<UserWhisperMessageHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<UserWhisperMessageHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}