using Newtonsoft.Json;

namespace TwitcheryNet.Net.EventSub.EventArgs;

public class EventSubNotification
{
    [JsonProperty("subscription")]
    public NotificationSubscription Subscription { get; set; } = new();

    [JsonProperty("event")]
    public object Event { get; set; } = new();
}

[JsonObject]
public class EventSubNotification<T> : EventSubNotification where T : class, new()
{
    [JsonProperty("event")]
    public new T Event { get; set; } = new();
}