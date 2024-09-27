using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.GuestStarGuest;

public class ChannelGuestStarGuestUpdateHandler : INotification
{
    public string SubscriptionType => "channel.guest_star_guest.update";
    public string SubscriptionVersion => "beta";
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}