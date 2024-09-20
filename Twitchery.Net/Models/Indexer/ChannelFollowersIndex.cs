using Microsoft.Extensions.DependencyInjection;
using TwitcheryNet.Attributes;
using TwitcheryNet.Models.Helix.Channels;
using TwitcheryNet.Services.Interfaces;

namespace TwitcheryNet.Models.Indexer;

public class ChannelFollowersIndex
{
    private ITwitchery Twitch { get; }
    
    [ActivatorUtilitiesConstructor]
    public ChannelFollowersIndex(ITwitchery api)
    {
        Twitch = api;
    }
    
    public List<Follower> this[string broadcasterId] => GetAllChannelFollowersAsync(broadcasterId).Result;
    
    [ApiRoute("GET", "channels/followers", "moderator:read:followers")]
    public async Task<GetChannelFollowersResponse?> GetChannelFollowersAsync(GetChannelFollowersRequest request, CancellationToken cancellationToken = default)
    {
        return await Twitch.GetTwitchApiAsync<GetChannelFollowersRequest, GetChannelFollowersResponse>(request, typeof(ChannelFollowersIndex), cancellationToken);
    }
    
    [ApiRoute("GET", "channels/followers", "moderator:read:followers")]
    public async Task<GetAllChannelFollowersResponse> GetAllChannelFollowersAsync(GetChannelFollowersRequest request, CancellationToken cancellationToken = default)
    {
        return await Twitch.GetTwitchApiAllAsync<GetChannelFollowersRequest, GetChannelFollowersResponse, GetAllChannelFollowersResponse>(request, typeof(ChannelFollowersIndex), cancellationToken);
    }
    
    public async Task<List<Follower>> GetChannelFollowersAsync(string broadcasterId, CancellationToken cancellationToken = default)
    {
        var followers = await GetChannelFollowersAsync(new GetChannelFollowersRequest(broadcasterId), cancellationToken);
        return followers?.Followers ?? [];
    }
    
    public async Task<List<Follower>> GetAllChannelFollowersAsync(string broadcasterId, CancellationToken cancellationToken = default)
    {
        var followers = await GetAllChannelFollowersAsync(new GetChannelFollowersRequest(broadcasterId), cancellationToken);
        return followers.Followers;
    }
}