using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.Moderation;

[JsonObject]
public class ModeratedChannel
{
    [JsonProperty("broadcaster_id")]
    public string BroadcasterId { get; set; } = string.Empty;
    
    [JsonProperty("broadcaster_login")]
    public string BroadcasterLogin { get; set; } = string.Empty;
    
    [JsonProperty("broadcaster_name")]
    public string BroadcasterName { get; set; } = string.Empty;
}