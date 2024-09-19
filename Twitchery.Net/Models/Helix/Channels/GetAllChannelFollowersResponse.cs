using System.Collections;
using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.Channels;

[JsonObject]
public class GetAllChannelFollowersResponse : IHasTotal, IFullResponse<GetChannelFollowersResponse>, IEnumerable<Follower>
{
    [JsonProperty("data")]
    public List<Follower> Followers { get; set; } = [];

    [JsonProperty("total")]
    public int Total { get; set; }

    public IEnumerator<Follower> GetEnumerator()
    {
        return Followers.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(GetChannelFollowersResponse item)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        
        Followers.AddRange(item.Followers);
    }
}