using Microsoft.Extensions.DependencyInjection;
using TwitcheryNet.Extensions.TwitchApi;
using TwitcheryNet.Models.Helix.Channels;
using TwitcheryNet.Services.Interfaces;

namespace TwitcheryNet.Models.Indexer;

public class ChannelInformationsIndex
{
    private ITwitchApiService Twitch { get; }
    
    [ActivatorUtilitiesConstructor]
    public ChannelInformationsIndex(ITwitchApiService api)
    {
        Twitch = api;
    }
    
    public ChannelInformation? this[string broadcasterId]
    {
        get
        {
            var channel = Twitch.GetChannelInformationAsync(broadcasterId).Result;
            return channel?.ChannelInformations.FirstOrDefault();
        }
    }

    public ChannelInformation? this[uint broadcasterId]
    {
        get
        {
            var channel = Twitch.GetChannelInformationAsync(broadcasterId.ToString()).Result;
            return channel?.ChannelInformations.FirstOrDefault();
        }
    }
}