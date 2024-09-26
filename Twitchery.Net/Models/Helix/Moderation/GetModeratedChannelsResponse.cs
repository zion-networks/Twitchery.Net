using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.Moderation;

[JsonObject]
public class GetModeratedChannelsResponse : IHasPagination
{
    [JsonProperty("data")]
    public List<ModeratedChannel> Data { get; set; } = [];
    
    [JsonProperty("pagination")]
    public Pagination Pagination { get; set; } = new();
}