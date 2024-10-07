using System.ComponentModel.DataAnnotations;
using TwitcheryNet.Attributes;

namespace TwitcheryNet.Models.Helix.Channels;

public class GetVipsRequest : IQueryParameters, IWithPagination
{
    [QueryParameter("broadcaster_id", true)]
    public string BroadcasterId { get; set; }
    
    [QueryParameter("user_id", false, true)]
    [MaxLength(100)]
    public List<string>? UserId { get; set; }
    
    [QueryParameter("first")]
    [Range(1, 100)]
    public int First { get; set; } = 20;

    [QueryParameter("after")]
    public string? After { get; set; }
    
    public GetVipsRequest(string broadcasterId)
    {
        BroadcasterId = broadcasterId;
    }
}