using TwitcheryNet.Attributes;

namespace TwitcheryNet.Models.Helix.Chat;

public class GetChattersRequest : IQueryParameters, IWithPagination
{
    [QueryParameter("broadcaster_id", true)]
    public string BroadcasterId { get; set; }
    
    [QueryParameter("moderator_id", true)]
    public string ModeratorId { get; set; }
    
    [QueryParameter("first")]
    public int? First { get; set; }
    
    [QueryParameter("after")]
    public string? After { get; set; }
    
    public GetChattersRequest(string broadcasterId, string moderatorId)
    {
        BroadcasterId = broadcasterId;
        ModeratorId = moderatorId;
    }
}