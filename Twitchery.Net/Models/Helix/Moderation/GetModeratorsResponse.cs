using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.Moderation;

[JsonObject]
public class GetModeratorsResponse : IHasPagination
{
    [JsonProperty("data")]
    public List<UserBase> Moderators { get; set; } = [];

    [JsonProperty("pagination")]
    public Pagination Pagination { get; set; } = new();
}