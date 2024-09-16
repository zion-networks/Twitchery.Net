using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.Channels;

[JsonObject]
public class GetChannelFollowersResponse : IHasPagination, IHasTotal
{
    [JsonProperty("data")]
    public List<Follower> Followers { get; set; } = [];
    
    [JsonProperty("pagination")]
    public Pagination Pagination { get; set; } = new();

    [JsonProperty("total")]
    public int Total { get; set; }
}