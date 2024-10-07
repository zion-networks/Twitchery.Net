using System.Collections;
using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.Chat;

[JsonObject]
public class GetAllChattersResponse : IHasTotal, IFullResponse<GetChattersResponse>, IEnumerable<UserBase>
{
    [JsonProperty("data")]
    public List<UserBase> Chatters { get; set; } = [];
    
    [JsonProperty("total")]
    public int Total { get; set; }
    
    public void Add(GetChattersResponse item)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        
        Chatters.AddRange(item.Chatters);
    }

    public IEnumerator<UserBase> GetEnumerator()
    {
        return Chatters.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}