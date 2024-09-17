using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.Streams;

[JsonObject]
public class Stream
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonProperty("user_id")]
    public string UserId { get; set; } = string.Empty;
    
    [JsonProperty("user_login")]
    public string UserLogin { get; set; } = string.Empty;
    
    [JsonProperty("user_name")]
    public string UserName { get; set; } = string.Empty;
    
    [JsonProperty("game_id")]
    public string GameId { get; set; } = string.Empty;
    
    [JsonProperty("game_name")]
    public string GameName { get; set; } = string.Empty;
    
    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonProperty("tags")]
    public List<string> Tags { get; set; } = [];
    
    [JsonProperty("viewer_count")]
    public int ViewerCount { get; set; }
    
    [JsonProperty("started_at")]
    public DateTime StartedAt { get; set; }
    
    [JsonProperty("language")]
    public string Language { get; set; } = string.Empty;
    
    [JsonProperty("thumbnail_url")]
    public string ThumbnailUrl { get; set; } = string.Empty;
    
    [JsonProperty("tag_ids")]
    public List<string> TagIds { get; set; } = [];
    
    [JsonProperty("is_mature")]
    public bool IsMature { get; set; }
}