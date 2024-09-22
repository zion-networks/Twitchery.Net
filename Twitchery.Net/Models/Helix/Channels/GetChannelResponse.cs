using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.Channels;

[JsonObject]
public class GetChannelResponse
{
    [JsonProperty("data")]
    public List<Channel> ChannelInformations { get; set; } = [];
}