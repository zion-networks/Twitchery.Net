using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.Polls;

[JsonObject]
public class PollChoice
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonProperty("title")]
    public string Title { get; set; }
    
    [JsonProperty("votes")]
    public int Votes { get; set; }
    
    [JsonProperty("channel_points_votes")]
    public int ChannelPointsVotes { get; set; }
    
    [JsonProperty("bits_votes")]
    public int BitsVotes { get; set; }
    
    public PollChoice(string title)
    {
        Title = title;
    }
    
    public static implicit operator PollChoice(string title) => new(title);
}