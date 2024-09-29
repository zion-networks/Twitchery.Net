using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Automod.Terms;

public class AutomodTermsUpdateHandler : INotification
{
    public string SubscriptionType => "automod.terms.update";
    public string SubscriptionVersion => "1";
    
    private ILogger<AutomodTermsUpdateHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<AutomodTermsUpdateHandler>();

    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}