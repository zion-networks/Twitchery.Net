using Newtonsoft.Json;

namespace TwitcheryNet.Models.Client;

[JsonObject]
public class WebSocketMessage
{
    [JsonProperty("metadata")]
    public MessageMetaData Metadata { get; set; } = new();
}