using Newtonsoft.Json;

namespace TwitcheryNet.Models.Client.Messages.Welcome;

[JsonObject]
public class SessionWelcomeMessage
{
    [JsonProperty("metadata")]
    public MessageMetaData Metadata { get; set; } = new();
    
    [JsonProperty("payload")]
    public SessionWelcomePayload Payload { get; set; } = new();
}