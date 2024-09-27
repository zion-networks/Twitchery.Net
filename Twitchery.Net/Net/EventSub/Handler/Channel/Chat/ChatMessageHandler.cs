using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TwitcheryNet.Net.EventSub.EventArgs.Channel;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.Chat;

public class ChatMessageHandler : INotification
{
    public string SubscriptionType => "channel.chat.message";
    
    public ILogger<ChatMessageHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChatMessageHandler>();
    
    public async Task Handle(EventSubClient client, string json)
    {
        try
        {
            var data = JsonConvert.DeserializeObject<EventSubNotificationData<ChatMessageNotification>>(json);

            if (data is null)
            {
                throw new JsonSerializationException(
                    $"Failed to deserialize JSON for {nameof(ChatMessageNotification)}");
            }

            await client.RaiseEventAsync(SubscriptionType, data);
        }
        catch
        {
            Logger.LogError("Failed to handle {SubscriptionType} notification", SubscriptionType);
        }
    }
}