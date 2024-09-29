using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Automod.Message;

public class AutomodMessageUpdateHandler : INotification
{
    public string SubscriptionType => "automod.message.update";
    public string SubscriptionVersion => "1";
    
    private ILogger<AutomodMessageUpdateHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<AutomodMessageUpdateHandler>();

    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}