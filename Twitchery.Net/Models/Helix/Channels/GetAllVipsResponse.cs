using System.Collections;
using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.Channels;

[JsonObject]
public class GetAllVipsResponse : IHasTotal, IFullResponse<GetVipsResponse>, IEnumerable<UserBase>
{
    [JsonProperty("data")]
    public List<UserBase> Vips { get; set; } = [];

    [JsonProperty("total")]
    public int Total { get; set; }

    public IEnumerator<UserBase> GetEnumerator()
    {
        return Vips.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(GetVipsResponse item)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        
        Vips.AddRange(item.Vips);
    }
}