using Newtonsoft.Json;
using TwitcheryNet.Attributes;

namespace TwitcheryNet.Models.Helix.Moderation;

[JsonObject]
public class GetModeratedChannelsRequest : IQueryParameters, IWithPagination
{
    [QueryParameter("user_id", true)]
    public string UserId { get; set; }
    
    [QueryParameter("after")]
    public string? After { get; set; }
    
    [QueryParameter("first")]
    public int First { get; set; } = 20;
    
    public GetModeratedChannelsRequest(string userId)
    {
        UserId = userId;
    }
}