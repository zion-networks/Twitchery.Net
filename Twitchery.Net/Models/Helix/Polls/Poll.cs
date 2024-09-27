using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.Polls;

[JsonObject]
public class Poll
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonProperty("broadcaster_id")]
    public string BroadcasterId { get; set; } = string.Empty;
    
    [JsonProperty("broadcaster_name")]
    public string BroadcasterName { get; set; } = string.Empty;
    
    [JsonProperty("broadcaster_login")]
    public string BroadcasterLogin { get; set; } = string.Empty;
    
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonProperty("choices")]
    public List<PollChoice> Choices { get; set; } = [];
    
    [JsonProperty("bits_voting_enabled")]
    public bool BitsVotingEnabled { get; set; }
    
    [JsonProperty("bits_per_vote")]
    public int BitsPerVote { get; set; }
    
    [JsonProperty("channel_points_voting_enabled")]
    public bool ChannelPointsVotingEnabled { get; set; }
    
    [JsonProperty("channel_points_per_vote")]
    public int ChannelPointsPerVote { get; set; }
    
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonProperty("duration")]
    public int Duration { get; set; }
    
    [JsonProperty("started_at")]
    public DateTime StartedAt { get; set; }
    
    [JsonProperty("ended_at")]
    public DateTime EndedAt { get; set; }
}