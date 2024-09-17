using TwitcheryNet.Attributes;

namespace TwitcheryNet.Models.Helix.Streams;

public class GetStreamsRequest : IQueryParameters
{
    [QueryParameter("user_id")]
    public List<string>? UserId { get; set; }
    
    [QueryParameter("user_login")]
    public List<string>? UserLogin { get; set; }
    
    [QueryParameter("game_id")]
    public List<string>? GameId { get; set; }
    
    [QueryParameter("type")]
    public string? Type { get; set; }
    
    [QueryParameter("language")]
    public List<string>? Language { get; set; }
    
    [QueryParameter("first")]
    public int? First { get; set; }
    
    [QueryParameter("before")]
    public string? Before { get; set; }
    
    [QueryParameter("after")]
    public string? After { get; set; }
}