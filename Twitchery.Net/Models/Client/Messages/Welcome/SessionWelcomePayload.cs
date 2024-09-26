using Newtonsoft.Json;

namespace TwitcheryNet.Models.Client.Messages.Welcome;

[JsonObject]
public class SessionWelcomePayload
{
    [JsonProperty("session")]
    public MessagePayloadSession Session { get; set; } = new();
}