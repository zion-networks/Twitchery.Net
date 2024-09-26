using Newtonsoft.Json;

namespace TwitcheryNet.Models.Client;

[JsonObject]
public class MessageMetaData
{
    [JsonProperty("message_id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonProperty("message_type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonProperty("message_timestamp")]
    public DateTime Timestamp { get; set; }
    
    [JsonProperty("subscription_type")]
    public string SubscriptionType { get; set; } = string.Empty;
    
    [JsonProperty("subscription_version")]
    public string SubscriptionVersion { get; set; } = string.Empty;
}