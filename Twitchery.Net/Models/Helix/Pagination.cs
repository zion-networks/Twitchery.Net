using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix;

public class Pagination
{
    [JsonProperty("cursor")]
    public string? Cursor { get; set; }
}