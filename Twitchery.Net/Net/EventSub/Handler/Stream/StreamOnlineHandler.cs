using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Stream;

public class StreamOnlineHandler : INotification
{
    public string SubscriptionType => "stream.online";
    public string SubscriptionVersion => "1";
    
    private ILogger<StreamOnlineHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<StreamOnlineHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}