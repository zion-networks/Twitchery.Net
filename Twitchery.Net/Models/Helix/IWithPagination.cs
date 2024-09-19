using TwitcheryNet.Attributes;

namespace TwitcheryNet.Models.Helix;

public interface IWithPagination
{
    [QueryParameter("after")]
    public string? After { get; set; }
}