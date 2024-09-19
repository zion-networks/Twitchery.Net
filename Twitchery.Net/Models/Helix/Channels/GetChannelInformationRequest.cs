using TwitcheryNet.Attributes;

namespace TwitcheryNet.Models.Helix.Channels;

public class GetChannelInformationRequest : IQueryParameters
{
    [QueryParameter("broadcaster_id", true, true)]
    public string BroadcasterId { get; set; }
    
    public GetChannelInformationRequest(string broadcasterId)
    {
        BroadcasterId = broadcasterId;
    }
}