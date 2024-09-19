using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.Chat.Messages;

[JsonObject]
public class SendChatMessageResponse
{
    [JsonProperty("data", Required = Required.Always)]
    public List<SentChatMessage> SentMessages { get; set; } = [];
}