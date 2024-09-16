using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.Chat;

[JsonObject]
public class GetChattersResponse : IHasPagination
{
    [JsonProperty("data")]
    public List<UserBase> Chatters { get; set; } = [];
    
    [JsonProperty("pagination")]
    public Pagination Pagination { get; set; } = new();
}