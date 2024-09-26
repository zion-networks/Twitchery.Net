using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.EventSub.Subscriptions;

[JsonObject]
public class EventSubTransport
{
    [JsonProperty("method")]
    public string Method { get; set; } = "websocket";
    
    [JsonProperty("callback")]
    public string? Callback { get; set; }
    
    [JsonProperty("secret")]
    public string? Secret { get; set; }
    
    [JsonProperty("session_id")]
    public string? SessionId { get; set; }
    
    [JsonProperty("conduit_id")]
    public string? ConduitId { get; set; }
}