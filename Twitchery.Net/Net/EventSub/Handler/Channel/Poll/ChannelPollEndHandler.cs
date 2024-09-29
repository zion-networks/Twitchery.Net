using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.Poll;

public class ChannelPollEndHandler : INotification
{
    public string SubscriptionType => "channel.poll.end";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelPollEndHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelPollEndHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}