using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.GuestStarSession;

public class ChannelGuestStarSessionEndHandler : INotification
{
    public string SubscriptionType => "channel.guest_star_session.end";
    public string SubscriptionVersion => "beta";
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}