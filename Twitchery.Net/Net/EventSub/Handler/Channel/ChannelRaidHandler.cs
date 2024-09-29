using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel;

public class ChannelRaidHandler : INotification
{
    public string SubscriptionType => "channel.raid";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelRaidHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelRaidHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        return Task.CompletedTask;
    }
}