using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.GuestStarSettings;

public class ChannelGuestStarSettingsUpdateHandler : INotification
{
    public string SubscriptionType => "channel.guest_star_settings.update";
    public string SubscriptionVersion => "beta";
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}