using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.Shoutout;

public class ChannelShoutoutReceiveHandler : INotification
{
    public string SubscriptionType => "channel.shoutout.receive";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelShoutoutReceiveHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelShoutoutReceiveHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}