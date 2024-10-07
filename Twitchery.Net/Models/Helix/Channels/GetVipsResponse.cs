using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.Channels;

[JsonObject]
public class GetVipsResponse : IHasPagination
{
    [JsonProperty("data")]
    public List<UserBase> Vips { get; set; } = [];
    
    [JsonProperty("pagination")]
    public Pagination Pagination { get; set; } = new();
}