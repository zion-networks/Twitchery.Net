using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.Chat.Messages;

[JsonObject]
public class SendChatMessageRequestBody
{
    [JsonProperty("broadcaster_id", Required = Required.Always)]
    public string BroadcasterId { get; set; }
    
    [JsonProperty("sender_id", Required = Required.Always)]
    public string SenderId { get; set; }
    
    [JsonProperty("message", Required = Required.Always)]
    [MaxLength(500)]
    public string Message { get; set; }
    
    [JsonProperty("reply_parent_message_id")]
    public string? ReplyParentMessageId { get; set; }
    
    public SendChatMessageRequestBody(string broadcasterId, string senderId, string message)
    {
        BroadcasterId = broadcasterId;
        SenderId = senderId;
        Message = message;
    }
}