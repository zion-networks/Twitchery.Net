using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix;

public interface IHasPagination
{
    [JsonProperty("pagination")]
    public Pagination Pagination { get; set; }
}