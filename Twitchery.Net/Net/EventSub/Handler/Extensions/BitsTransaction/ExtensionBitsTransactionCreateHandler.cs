using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Extensions.BitsTransaction;

public class ExtensionBitsTransactionCreateHandler : INotification
{
    public string SubscriptionType => "extension.bits_transaction.create";
    public string SubscriptionVersion => "1";
    
    private ILogger<ExtensionBitsTransactionCreateHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ExtensionBitsTransactionCreateHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}