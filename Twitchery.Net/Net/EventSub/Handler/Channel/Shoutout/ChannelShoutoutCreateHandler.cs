using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.Shoutout;

public class ChannelShoutoutCreateHandler : INotification
{
    public string SubscriptionType => "channel.shoutout.create";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelShoutoutCreateHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelShoutoutCreateHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}