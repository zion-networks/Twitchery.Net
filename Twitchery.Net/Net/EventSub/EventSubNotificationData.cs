using Newtonsoft.Json;
using TwitcheryNet.Models.Client;
using TwitcheryNet.Net.EventSub.EventArgs;

namespace TwitcheryNet.Net.EventSub;

[JsonObject]
public class EventSubNotificationData<T> where T : class, new()
{
    [JsonProperty("metadata")]
    public MessageMetaData Metadata { get; set; } = new();
    
    [JsonProperty("payload")]
    public EventSubNotification<T> Payload { get; set; } = new();
}

[JsonObject]
public class EventSubNotificationData
{
    [JsonProperty("metadata")]
    public MessageMetaData Metadata { get; set; } = new();
    
    [JsonProperty("payload")]
    public EventSubNotification Payload { get; set; } = new();
}