using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.Poll;

public class ChannelPollProgressHandler : INotification
{
    public string SubscriptionType => "channel.poll.progress";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelPollProgressHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelPollProgressHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}