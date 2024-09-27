using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Automod.Settings;

public class AutomodSettingsUpdateHandler : INotification
{
    public string SubscriptionType => "automod.settings.update";
    public string SubscriptionVersion => "1";

    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}