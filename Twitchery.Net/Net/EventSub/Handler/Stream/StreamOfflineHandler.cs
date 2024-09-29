using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Stream;

public class StreamOfflineHandler : INotification
{
    public string SubscriptionType => "stream.offline";
    public string SubscriptionVersion => "1";
    
    private ILogger<StreamOfflineHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<StreamOfflineHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}