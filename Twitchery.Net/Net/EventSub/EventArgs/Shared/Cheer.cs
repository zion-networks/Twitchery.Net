using Newtonsoft.Json;

namespace TwitcheryNet.Net.EventSub.EventArgs.Shared;

[JsonObject]
public class Cheer
{
    [JsonProperty("bits")]
    public int Bits { get; set; }
}