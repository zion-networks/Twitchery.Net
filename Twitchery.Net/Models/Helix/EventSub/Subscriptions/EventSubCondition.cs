using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using TwitcheryNet.Extensions;
using TwitcheryNet.Net.EventSub.EventArgs;

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

    public string ToSha256(params string[] prefixes)
    {
        if (BroadcasterUserId.IsNullOrWhiteSpace() && ModeratorUserId.IsNullOrWhiteSpace() && UserId.IsNullOrWhiteSpace())
        {
            throw new InvalidOperationException("At least one of the following properties must be set: BroadcasterUserId, ModeratorUserId, UserId");
        }
        
        var prefix = string.Join("", prefixes);
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes($"{prefix}{BroadcasterUserId}{ModeratorUserId}{UserId}"));

        return string.Join("", hash.Select(b => b.ToString("x2")));
    }
}