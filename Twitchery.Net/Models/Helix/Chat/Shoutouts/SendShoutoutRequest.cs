using Newtonsoft.Json;
using TwitcheryNet.Attributes;

namespace TwitcheryNet.Models.Helix.Chat.Shoutouts;

[JsonObject]
public class SendShoutoutRequest : IQueryParameters
{
    [JsonProperty("from_broadcaster_id")]
    [QueryParameter("from_broadcaster_id", true)]
    public string FromBroadcasterId { get; set; }
    
    [JsonProperty("to_broadcaster_id")]
    [QueryParameter("to_broadcaster_id", true)]
    public string ToBroadcasterId { get; set; }
    
    [JsonProperty("moderator_id")]
    [QueryParameter("moderator_id", true)]
    public string ModeratorId { get; set; }
    
    public SendShoutoutRequest(string fromBroadcasterId, string toBroadcasterId, string moderatorId)
    {
        FromBroadcasterId = fromBroadcasterId;
        ToBroadcasterId = toBroadcasterId;
        ModeratorId = moderatorId;
    }
}