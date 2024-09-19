using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.Users;

[JsonObject]
public class GetUsersResponse
{
    [JsonProperty("data")]
    public List<User> Users { get; set; } = [];
}