using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.EventSub.Subscriptions;

[JsonObject]
public class EventSubCondition
{
    [JsonProperty("broadcaster_user_id")]
    public string? BroadcasterUserId { get; set; }
    
    [JsonProperty("moderator_user_id")]
    public string? ModeratorUserId { get; set; }
    
    [JsonProperty("user_id")]
    public string? UserId { get; set; }
    
    public EventSubCondition(string? broadcasterUserId = null, string? moderatorUserId = null, string? userId = null)
    {
        BroadcasterUserId = broadcasterUserId;
        ModeratorUserId = moderatorUserId;
        UserId = userId;
    }
}