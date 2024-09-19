using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.Chat.Messages;

[JsonObject]
public class SentChatMessage
{
    [JsonProperty("message_id", Required = Required.Always)]
    public string MessageId { get; set; } = string.Empty;
    
    [JsonProperty("is_sent", Required = Required.Always)]
    public bool IsSent { get; set; }
    
    [JsonProperty("drop_reason", Required = Required.AllowNull)]
    public SentChatMessageDropReason? DropReason { get; set; }
}