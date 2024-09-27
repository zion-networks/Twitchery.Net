using Newtonsoft.Json;

namespace TwitcheryNet.Net.EventSub.EventArgs.Generic;

[JsonObject]
public class Cheer
{
    [JsonProperty("bits")]
    public int Bits { get; set; }
}