using Newtonsoft.Json;
using TwitcheryNet.Net.EventSub.EventArgs.Generic;

namespace TwitcheryNet.Net.EventSub.EventArgs.Channel;

[JsonObject]
public class ChatMessageNotification
{
    [JsonProperty("broadcaster_user_id")]
    public string BroadcasterUserId { get; set; } = string.Empty;
    
    [JsonProperty("broadcaster_user_login")]
    public string BroadcasterUserLogin { get; set; } = string.Empty;
    
    [JsonProperty("broadcaster_user_name")]
    public string BroadcasterUserName { get; set; } = string.Empty;
    
    [JsonProperty("chatter_user_id")]
    public string ChatterUserId { get; set; } = string.Empty;
    
    [JsonProperty("chatter_user_login")]
    public string ChatterUserLogin { get; set; } = string.Empty;
    
    [JsonProperty("chatter_user_name")]
    public string ChatterUserName { get; set; } = string.Empty;
    
    [JsonProperty("message_id")]
    public string MessageId { get; set; } = string.Empty;
    
    [JsonProperty("message")]
    public Message Message { get; set; } = new();
    
    [JsonProperty("color")]
    public string Color { get; set; } = string.Empty;
    
    [JsonProperty("badges")]
    public List<Badge> Badges { get; set; } = [];
    
    [JsonProperty("message_type")]
    public string MessageType { get; set; } = string.Empty;
    
    [JsonProperty("cheer")]
    public Cheer? Cheer { get; set; }
    
    [JsonProperty("reply")]
    public Reply? Reply { get; set; }
    
    [JsonProperty("channel_points_custom_reward_id")]
    public string? ChannelPointsCustomRewardId { get; set; }
    
    [JsonProperty("source_broadcaster_user_id")]
    public string? SourceBroadcasterUserId { get; set; }
    
    [JsonProperty("source_broadcaster_user_login")]
    public string? SourceBroadcasterUserLogin { get; set; }
    
    [JsonProperty("source_broadcaster_user_name")]
    public string? SourceBroadcasterUserName { get; set; }
    
    [JsonProperty("source_message_id")]
    public string? SourceMessageId { get; set; }
    
    [JsonProperty("source_badges")]
    public List<Badge>? SourceBadges { get; set; }
}