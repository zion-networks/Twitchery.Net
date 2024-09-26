using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.EventSub.Subscriptions;

[JsonObject]
public class CreateEventSubSubscriptionRequestBody
{
    [JsonProperty("type")]
    public string Type { get; set; }
    
    [JsonProperty("version")]
    public string Version { get; set; }
    
    [JsonProperty("condition")]
    public EventSubCondition Condition { get; set; } = new();
    
    [JsonProperty("transport")]
    public EventSubTransport Transport { get; set; } = new();

    public CreateEventSubSubscriptionRequestBody(string type, string version)
    {
        Type = type;
        Version = version;
    }
}