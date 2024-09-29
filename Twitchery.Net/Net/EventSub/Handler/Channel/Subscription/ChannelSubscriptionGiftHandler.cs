using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.Subscription;

public class ChannelSubscriptionGiftHandler : INotification
{
    public string SubscriptionType => "channel.subscription.gift";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelSubscriptionGiftHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelSubscriptionGiftHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        return Task.CompletedTask;
    }
}