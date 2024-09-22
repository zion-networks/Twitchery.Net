using TwitcheryNet.Attributes;

namespace TwitcheryNet.Models.Helix.Channels;

public class GetChannelRequest : IQueryParameters
{
    [QueryParameter("broadcaster_id", true, true)]
    public string BroadcasterId { get; set; }
    
    public GetChannelRequest(string broadcasterId)
    {
        BroadcasterId = broadcasterId;
    }
}