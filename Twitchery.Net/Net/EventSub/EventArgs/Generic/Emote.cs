using Newtonsoft.Json;

namespace TwitcheryNet.Net.EventSub.EventArgs.Generic;

[JsonObject]
public class Emote
{
    public string Id { get; set; } = string.Empty;
    public string EmoteSetId { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public List<string> Format { get; set; } = [];
}