using Newtonsoft.Json;

namespace TwitcheryNet.Net.EventSub.EventArgs.Generic;

[JsonObject]
public class Cheermote
{
    [JsonProperty("prefix")]
    public string Prefix { get; set; } = string.Empty;
    
    [JsonProperty("bits")]
    public int Bits { get; set; }
    
    [JsonProperty("tier")]
    public int Tier { get; set; }
}