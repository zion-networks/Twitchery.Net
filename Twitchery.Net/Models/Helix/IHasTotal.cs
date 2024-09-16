using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix;

public interface IHasTotal
{
    [JsonProperty("total")]
    public int Total { get; set; }
}