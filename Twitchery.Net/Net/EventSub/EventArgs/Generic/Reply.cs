using Newtonsoft.Json;

namespace TwitcheryNet.Net.EventSub.EventArgs.Generic;

[JsonObject]
public class Reply
{
    [JsonProperty("parent_message_id")]
    public string ParentMessageId { get; set; } = string.Empty;
    
    [JsonProperty("parent_message_body")]
    public string ParentMessageBody { get; set; } = string.Empty;
    
    [JsonProperty("parent_user_id")]
    public string ParentUserId { get; set; } = string.Empty;
    
    [JsonProperty("parent_user_name")]
    public string ParentUserName { get; set; } = string.Empty;
    
    [JsonProperty("parent_user_login")]
    public string ParentUserLogin { get; set; } = string.Empty;
    
    [JsonProperty("thread_message_id")]
    public string ThreadMessageId { get; set; } = string.Empty;
    
    [JsonProperty("thread_user_id")]
    public string ThreadUserId { get; set; } = string.Empty;
    
    [JsonProperty("thread_user_name")]
    public string ThreadUserName { get; set; } = string.Empty;
    
    [JsonProperty("thread_user_login")]
    public string ThreadUserLogin { get; set; } = string.Empty;
}