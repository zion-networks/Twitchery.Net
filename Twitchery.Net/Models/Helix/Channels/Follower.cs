using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.Channels;

[JsonObject]
public class Follower : UserBase
{
    [JsonProperty("followed_at")]
    public DateTime FollowedAt { get; set; }
}