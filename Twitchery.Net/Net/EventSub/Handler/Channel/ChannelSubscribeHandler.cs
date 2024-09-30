using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TwitcheryNet.Misc;
using TwitcheryNet.Net.EventSub.EventArgs.Channel;

namespace TwitcheryNet.Net.EventSub.Handler.Channel;

public class ChannelSubscribeHandler : INotification
{
    public string SubscriptionType => "channel.subscribe";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelSubscribeHandler> Logger { get; } = 
        LoggerFactory
            .Create(x => x.AddConsole())
            .CreateLogger<ChannelSubscribeHandler>();
    
    public async Task Handle(EventSubClient client, string json)
    {
        try
        {
            var data = JsonConvert.DeserializeObject<EventSubNotificationData<ChannelSubscribeNotification>>(json);

            if (data is null)
            {
                throw new JsonSerializationException(
                    $"Failed to deserialize JSON for {nameof(ChannelSubscribeNotification)}");
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