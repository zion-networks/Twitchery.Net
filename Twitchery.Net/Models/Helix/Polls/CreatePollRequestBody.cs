using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TwitcheryNet.Models.Helix.Polls;

[JsonObject]
public class CreatePollRequestBody
{
    [JsonProperty("broadcaster_id")]
    public string BroadcasterId { get; set; }
    
    [JsonProperty("title")]
    public string Title { get; set; }
    
    [JsonProperty("choices")]
    [MinLength(1), MaxLength(5)]
    public PollChoice[] Choices { get; set; } = [];

    [JsonProperty("duration")]
    [Range(15, 1800, MinimumIsExclusive = false, MaximumIsExclusive = false)]
    public int Duration { get; set; } = 15;
    
    [JsonProperty("channel_points_voting_enabled")]
    public bool ChannelPointsVotingEnabled { get; set; }
    
    [JsonProperty("channel_points_per_vote")]
    [Range(1, 1000000, MinimumIsExclusive = false, MaximumIsExclusive = false)]
    public int ChannelPointsPerVote { get; set; } = 1;
    
    public CreatePollRequestBody(string broadcasterId, string title, params PollChoice[] choices)
    {
        if (choices.Length is < 1 or > 5)
            throw new ArgumentOutOfRangeException(nameof(choices), "Choices must be between 1 and 5.");
        
        BroadcasterId = broadcasterId;
        Title = title;
        Choices = choices;
    }
}