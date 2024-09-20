using TwitcheryNet.Attributes;

namespace TwitcheryNet.Models.Helix.Streams;

public class GetStreamsRequest : IQueryParameters, IWithPagination
{
    [QueryParameter("user_id", false, true)]
    public List<string>? UserIds { get; set; }
    
    [QueryParameter("user_login", false, true)]
    public List<string>? UserLogins { get; set; }
    
    [QueryParameter("game_id", false, true)]
    public List<string>? GameIds { get; set; }
    
    [QueryParameter("type")]
    public string? Type { get; set; }
    
    [QueryParameter("language", false, true)]
    public List<string>? Languages { get; set; }
    
    [QueryParameter("first")]
    public int? First { get; set; }
    
    [QueryParameter("before")]
    public string? Before { get; set; }
    
    [QueryParameter("after")]
    public string? After { get; set; }
}