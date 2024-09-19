using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.Channels;

[JsonObject]
public class GetChannelInformationResponse
{
    [JsonProperty("data")]
    public List<ChannelInformation> ChannelInformations { get; set; } = [];
}