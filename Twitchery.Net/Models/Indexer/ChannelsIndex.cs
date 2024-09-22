using Microsoft.Extensions.DependencyInjection;
using TwitcheryNet.Attributes;
using TwitcheryNet.Models.Helix.Channels;
using TwitcheryNet.Services.Interfaces;

namespace TwitcheryNet.Models.Indexer;

public class ChannelsIndex
{
    private ITwitchery Twitch { get; }
    
    [ActivatorUtilitiesConstructor]
    public ChannelsIndex(ITwitchery api)
    {
        Twitch = api;
    }
    
    public Channel? this[string broadcasterId] => GetChannelInformationAsync(broadcasterId).Result;
    
    [ApiRoute("GET", "channels")]
    public async Task<GetChannelResponse?> GetChannelInformationAsync(GetChannelRequest request, CancellationToken cancellationToken = default)
    {
        return await Twitch.GetTwitchApiAsync<GetChannelRequest, GetChannelResponse>(request, typeof(ChannelsIndex), cancellationToken);
    }
    
    public async Task<Channel?> GetChannelInformationAsync(string broadcasterId, CancellationToken cancellationToken = default)
    {
        var channel = await GetChannelInformationAsync(new GetChannelRequest(broadcasterId), cancellationToken);
        return channel?.ChannelInformations.FirstOrDefault();
    }
}