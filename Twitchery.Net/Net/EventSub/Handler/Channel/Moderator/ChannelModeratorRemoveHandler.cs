using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.Moderator;

public class ChannelModeratorRemoveHandler : INotification
{
    public string SubscriptionType => "channel.moderator.remove";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelModeratorRemoveHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelModeratorRemoveHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}