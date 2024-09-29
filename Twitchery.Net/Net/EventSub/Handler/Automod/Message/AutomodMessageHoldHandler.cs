using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Automod.Message;

public class AutomodMessageHoldHandler : INotification
{
    public string SubscriptionType => "automod.message.hold";
    public string SubscriptionVersion => "1";
    
    private ILogger<AutomodMessageHoldHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<AutomodMessageHoldHandler>();

    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}