using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.Chat.Messages;

[JsonObject]
public class SentChatMessageDropReason
{
    [JsonProperty("code", Required = Required.Always)]
    public string Code { get; set; } = string.Empty;
    
    [JsonProperty("message", Required = Required.Always)]
    public string Message { get; set; } = string.Empty;
}