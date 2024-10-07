using System.ComponentModel.DataAnnotations;
using TwitcheryNet.Attributes;

namespace TwitcheryNet.Models.Helix.Moderation;

public class GetModeratorsRequest : IQueryParameters, IWithPagination
{
    [QueryParameter("broadcaster_id", true)]
    public string BroadcasterId { get; set; }
    
    [QueryParameter("user_id", false, true)]
    [MaxLength(100)]
    public List<string>? UserIds { get; set; }

    [QueryParameter("user_login")]
    [Range(1, 100)]
    public int First { get; set; } = 20;
    
    [QueryParameter("after")]
    public string? After { get; set; }
    
    public GetModeratorsRequest(string broadcasterId)
    {
        BroadcasterId = broadcasterId;
    }
}