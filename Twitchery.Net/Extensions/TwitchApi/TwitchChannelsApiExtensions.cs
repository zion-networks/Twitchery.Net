using TwitcheryNet.Attributes;
using TwitcheryNet.Models.Helix.Channels;
using TwitcheryNet.Services.Interfaces;

namespace TwitcheryNet.Extensions.TwitchApi;

public static class TwitchChannelsApiExtensions
{
    [ApiRoute("GET", "channels/followers", "moderator:read:followers")]
    public static async Task<GetChannelFollowersResponse?> GetChannelFollowersAsync(this ITwitchApiService service, string broadcasterId, string? userId = null, int? first = null, string? after = null, CancellationToken cancellationToken = default)
    {
        var request = new GetChannelFollowersRequest(broadcasterId)
        {
            UserId = userId,
            First = first,
            After = after
        };
        
        return await service.GetTwitchApiAsync<GetChannelFollowersRequest, GetChannelFollowersResponse>(request, typeof(TwitchChannelsApiExtensions), cancellationToken);
    }
    
    [ApiRoute("GET", "channels/followers", "moderator:read:followers")]
    public static async Task<GetAllChannelFollowersResponse> GetAllChannelFollowersAsync(this ITwitchApiService service, string broadcasterId, string? userId = null, int? first = null, CancellationToken cancellationToken = default)
    {
        var request = new GetChannelFollowersRequest(broadcasterId)
        {
            UserId = userId,
            First = first
        };
        
        var responses = await service.GetTwitchApiAllAsync<GetChannelFollowersRequest, GetChannelFollowersResponse, GetAllChannelFollowersResponse>(request, typeof(TwitchChannelsApiExtensions), cancellationToken);

        return responses;
    }
    
    [ApiRoute("GET", "channels")]
    public static async Task<GetChannelInformationResponse?> GetChannelInformationAsync(this ITwitchApiService service, string broadcasterId, CancellationToken cancellationToken = default)
    {
        var request = new GetChannelInformationRequest(broadcasterId);
        
        return await service.GetTwitchApiAsync<GetChannelInformationRequest, GetChannelInformationResponse>(request, typeof(TwitchChannelsApiExtensions), cancellationToken);
    }
}