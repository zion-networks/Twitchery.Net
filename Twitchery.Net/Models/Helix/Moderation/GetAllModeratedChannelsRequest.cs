using System.Collections;
using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.Moderation;

public class GetAllModeratedChannelsRequest : IHasTotal, IFullResponse<GetModeratedChannelsResponse>, IEnumerable<ModeratedChannel>
{
    [JsonProperty("data")]
    public List<ModeratedChannel> Channels { get; set; } = [];

    [JsonProperty("total")]
    public int Total { get; set; }

    public IEnumerator<ModeratedChannel> GetEnumerator()
    {
        return Channels.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(GetModeratedChannelsResponse item)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        
        Channels.AddRange(item.Data);
    }
}