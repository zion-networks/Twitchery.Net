using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.EventSub.Subscriptions;

[JsonObject]
public class CreateEventSubSubscriptionData
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonProperty("version")]
    public string Version { get; set; } = string.Empty;
    
    [JsonProperty("condition")]
    public EventSubCondition Condition { get; set; } = new();
    
    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [JsonProperty("transport")]
    public EventSubTransport Transport { get; set; } = new();
    
    [JsonProperty("cost")]
    public int Cost { get; set; }
    
    public string ToSha256()
    {
        return Condition.ToSha256(Type, Version);
    }
}