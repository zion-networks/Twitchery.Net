using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.Moderator;

public class ChannelModeratorAddHandler : INotification
{
    public string SubscriptionType => "channel.moderator.add";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelModeratorAddHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelModeratorAddHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}