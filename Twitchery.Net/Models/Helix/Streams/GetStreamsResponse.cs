using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.Streams;

[JsonObject]
public class GetStreamsResponse : IHasPagination
{
    [JsonProperty("data")]
    public List<Stream> Streams { get; set; } = [];
    
    [JsonProperty("pagination")]
    public Pagination Pagination { get; set; } = new();
}