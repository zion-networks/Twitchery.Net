using TwitcheryNet.Attributes;

namespace TwitcheryNet.Models.Helix.Channels;

public class GetChannelFollowersRequest : IQueryParameters, IWithPagination
{
    [QueryParameter("broadcaster_id", true)]
    public string BroadcasterId { get; set; }
    
    [QueryParameter("user_id")]
    public string? UserId { get; set; }
    
    [QueryParameter("first")]
    public int? First { get; set; }

    [QueryParameter("after")]
    public string? After { get; set; }
    
    public GetChannelFollowersRequest(string broadcasterId)
    {
        BroadcasterId = broadcasterId;
    }
}