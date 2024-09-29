using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel;

public class ChannelCheerHandler : INotification
{
    public string SubscriptionType => "channel.cheer";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelCheerHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelCheerHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        return Task.CompletedTask;
    }
}