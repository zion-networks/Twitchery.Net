using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TwitcheryNet.Net.EventSub.EventArgs.Channel;

namespace TwitcheryNet.Net.EventSub.Handler.Channel;

public class ChannelFollowHandler : INotification
{
    public string SubscriptionType => "channel.follow";
    public string SubscriptionVersion => "2";
    
    private ILogger<ChannelFollowNotification> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelFollowNotification>();
    
    public async Task Handle(EventSubClient client, string json)
    {
        try
        {
            var data = JsonConvert.DeserializeObject<EventSubNotificationData<ChannelFollowNotification>>(json);

            if (data is null)
            {
                throw new JsonSerializationException(
                    $"Failed to deserialize JSON for {nameof(ChannelFollowNotification)}");
            }

            var eventPath = $"{SubscriptionType}/{data.Payload.Event.BroadcasterUserId}";
            
            await client.RaiseEventAsync(eventPath, data);
        }
        catch
        {
            Logger.LogError("Failed to handle {SubscriptionType} notification", SubscriptionType);
        }
    }
}