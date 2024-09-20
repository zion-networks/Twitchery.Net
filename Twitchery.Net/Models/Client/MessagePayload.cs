using Newtonsoft.Json;

namespace TwitcheryNet.Models.Client;

[JsonObject]
public class MessagePayload
{
    [JsonProperty("session")]
    public MessagePayloadSession Session { get; set; } = new();
}