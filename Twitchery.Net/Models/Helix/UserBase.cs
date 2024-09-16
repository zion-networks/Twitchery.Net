using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix;

[JsonObject]
public class UserBase
{
    [JsonProperty("user_id")]
    public string UserId { get; set; } = string.Empty;
    
    [JsonProperty("user_login")]
    public string UserLogin { get; set; } = string.Empty;
    
    [JsonProperty("user_name")]
    public string UserName { get; set; } = string.Empty;
}