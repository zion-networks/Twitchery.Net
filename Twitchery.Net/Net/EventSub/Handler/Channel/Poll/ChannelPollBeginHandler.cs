using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.Poll;

public class ChannelPollBeginHandler : INotification
{
    public string SubscriptionType => "channel.poll.begin";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelPollBeginHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelPollBeginHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}