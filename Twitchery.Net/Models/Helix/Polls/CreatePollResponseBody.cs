using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.Polls;

[JsonObject]
public class CreatePollResponseBody
{
    [JsonProperty("data")]
    public List<Poll> Polls { get; set; } = [];
}