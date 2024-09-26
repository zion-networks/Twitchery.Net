using Newtonsoft.Json;

namespace TwitcheryNet.Net.EventSub.EventArgs.Generic;

[JsonObject]
public class Mention
{
    [JsonProperty("user_id")]
    public string UserId { get; set; } = string.Empty;
    
    [JsonProperty("user_name")]
    public string UserName { get; set; } = string.Empty;
    
    [JsonProperty("user_login")]
    public string UserLogin { get; set; } = string.Empty;
}