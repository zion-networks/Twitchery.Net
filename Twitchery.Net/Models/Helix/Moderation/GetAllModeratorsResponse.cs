using System.Collections;
using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.Moderation;

[JsonObject]
public class GetAllModeratorsResponse : IHasTotal, IFullResponse<GetModeratorsResponse>, IEnumerable<UserBase>
{
    [JsonProperty("data")]
    public List<UserBase> Moderators { get; set; } = [];
    
    [JsonProperty("total")]
    public int Total { get; set; }
    
    public void Add(GetModeratorsResponse item)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        
        Moderators.AddRange(item.Moderators);
    }

    public IEnumerator<UserBase> GetEnumerator()
    {
        return Moderators.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}