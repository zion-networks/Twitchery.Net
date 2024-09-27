using Newtonsoft.Json;

namespace TwitcheryNet.Net.EventSub.EventArgs.Generic;

[JsonObject]
public class Badge
{
    [JsonProperty("set_id")]
    public string SetId { get; set; } = string.Empty;

    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("info")]
    public string Info { get; set; } = string.Empty;
}