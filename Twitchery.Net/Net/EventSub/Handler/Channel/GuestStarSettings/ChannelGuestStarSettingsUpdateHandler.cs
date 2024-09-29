using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.GuestStarSettings;

public class ChannelGuestStarSettingsUpdateHandler : INotification
{
    public string SubscriptionType => "channel.guest_star_settings.update";
    public string SubscriptionVersion => "beta";
    
    private ILogger<ChannelGuestStarSettingsUpdateHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelGuestStarSettingsUpdateHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}