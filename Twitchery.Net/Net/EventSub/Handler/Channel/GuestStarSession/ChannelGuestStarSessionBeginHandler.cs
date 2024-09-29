using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.GuestStarSession;

public class ChannelGuestStarSessionBeginHandler : INotification
{
    public string SubscriptionType => "channel.guest_star_session.begin";
    public string SubscriptionVersion => "beta";
    
    private ILogger<ChannelGuestStarSessionBeginHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelGuestStarSessionBeginHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}