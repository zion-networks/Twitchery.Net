using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.EventSub.Subscriptions;

[JsonObject]
public class CreateEventSubSubscriptionResponseBody : IHasTotal
{
    [JsonProperty("data")]
    public List<CreateEventSubSubscriptionData> Data { get; set; } = [];
    
    [JsonProperty("connected_at")]
    public DateTime ConnectedAt { get; set; }
    
    [JsonProperty("total")]
    public int Total { get; set; }
    
    [JsonProperty("total_cost")]
    public int TotalCost { get; set; }
    
    [JsonProperty("max_total_cost")]
    public int MaxTotalCost { get; set; }
}