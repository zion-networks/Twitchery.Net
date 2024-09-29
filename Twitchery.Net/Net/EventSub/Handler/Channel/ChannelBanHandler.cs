using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel;

public class ChannelBanHandler : INotification
{
    public string SubscriptionType => "channel.ban";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelBanHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelBanHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        return Task.CompletedTask;
    }
}