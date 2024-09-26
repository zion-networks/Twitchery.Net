using Newtonsoft.Json;

namespace TwitcheryNet.Net.EventSub.EventArgs.Generic;

[JsonObject]
public class Fragment
{
    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("text")]
    public string Text { get; set; } = string.Empty;

    [JsonProperty("cheermote")]
    public Cheermote? Cheermote { get; set; }

    [JsonProperty("emote")]
    public Emote? Emote { get; set; }

    [JsonProperty("mention")]
    public Mention? Mention { get; set; }
}