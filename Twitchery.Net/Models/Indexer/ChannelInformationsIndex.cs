using Microsoft.Extensions.DependencyInjection;
using TwitcheryNet.Attributes;
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
    
    public ChannelInformation? this[string broadcasterId] => GetChannelInformationAsync(broadcasterId).Result;
    
    [ApiRoute("GET", "channels")]
    public async Task<GetChannelInformationResponse?> GetChannelInformationAsync(GetChannelInformationRequest request, CancellationToken cancellationToken = default)
    {
        return await Twitch.GetTwitchApiAsync<GetChannelInformationRequest, GetChannelInformationResponse>(request, typeof(ChannelInformationsIndex), cancellationToken);
    }
    
    public async Task<ChannelInformation?> GetChannelInformationAsync(string broadcasterId, CancellationToken cancellationToken = default)
    {
        var channel = await GetChannelInformationAsync(new GetChannelInformationRequest(broadcasterId), cancellationToken);
        return channel?.ChannelInformations.FirstOrDefault();
    }
}