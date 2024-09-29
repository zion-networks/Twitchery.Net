using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Conduit.Shard;

public class ConduitShardDisabledHandler : INotification
{
    public string SubscriptionType => "conduit.shard.disabled";
    public string SubscriptionVersion => "1";
    
    private ILogger<ConduitShardDisabledHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ConduitShardDisabledHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}