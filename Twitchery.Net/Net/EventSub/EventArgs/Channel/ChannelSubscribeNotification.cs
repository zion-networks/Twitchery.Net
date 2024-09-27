using Newtonsoft.Json;
using TwitcheryNet.Services.Interfaces;

namespace TwitcheryNet.Net.EventSub.EventArgs.Channel;

[JsonObject]
public class ChannelSubscribeNotification : IHasTwitchery
{
    [JsonIgnore]
    public ITwitchery? Twitch { get; set; }
    
    [JsonProperty("user_id")]
    public string UserId { get; set; } = string.Empty;
    
    [JsonProperty("user_login")]
    public string UserLogin { get; set; } = string.Empty;
    
    [JsonProperty("user_name")]
    public string UserName { get; set; } = string.Empty;
    
    [JsonProperty("broadcaster_user_id")]
    public string BroadcasterUserId { get; set; } = string.Empty;
    
    [JsonProperty("broadcaster_user_login")]
    public string BroadcasterUserLogin { get; set; } = string.Empty;
    
    [JsonProperty("broadcaster_user_name")]
    public string BroadcasterUserName { get; set; } = string.Empty;
    
    [JsonProperty("tier")]
    public string Tier { get; set; } = string.Empty;
    
    [JsonProperty("is_gift")]
    public bool IsGift { get; set; }
}