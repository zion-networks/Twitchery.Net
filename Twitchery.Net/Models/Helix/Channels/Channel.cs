using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.Channels;

[JsonObject]
public class Channel
{
    [JsonProperty("broadcaster_id")]
    public string BroadcasterId { get; set; } = string.Empty;
    
    [JsonProperty("broadcaster_login")]
    public string BroadcasterLogin { get; set; } = string.Empty;
    
    [JsonProperty("broadcaster_name")]
    public string BroadcasterName { get; set; } = string.Empty;
    
    [JsonProperty("broadcaster_language")]
    public string BroadcasterLanguage { get; set; } = string.Empty;
    
    [JsonProperty("game_name")]
    public string GameName { get; set; } = string.Empty;
    
    [JsonProperty("game_id")]
    public string GameId { get; set; } = string.Empty;
    
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonProperty("delay")]
    public uint Delay { get; set; }
    
    [JsonProperty("tags")]
    public List<string> Tags { get; set; } = [];
    
    [JsonProperty("content_classification_labels")]
    public List<string> ContentClassificationLabels { get; set; } = [];
    
    [JsonProperty("is_branded_content")]
    public bool IsBrandedContent { get; set; }
}