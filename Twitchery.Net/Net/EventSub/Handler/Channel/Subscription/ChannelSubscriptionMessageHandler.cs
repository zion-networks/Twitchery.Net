using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.Subscription;

public class ChannelSubscriptionMessageHandler : INotification
{
    public string SubscriptionType => "channel.subscription.message";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelSubscriptionMessageHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelSubscriptionMessageHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        return Task.CompletedTask;
    }
}