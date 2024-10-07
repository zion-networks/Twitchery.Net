using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using TwitcheryNet.Attributes;
using TwitcheryNet.Models.Helix;
using TwitcheryNet.Models.Helix.Channels;
using TwitcheryNet.Models.Helix.Moderation;
using TwitcheryNet.Models.Helix.Users;
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
    [RequiresToken(TokenType.Both)]
    public async Task<GetChannelResponse?> GetChannelInformationAsync(GetChannelRequest request, CancellationToken cancellationToken = default)
    {
        return await Twitch.GetTwitchApiAsync<GetChannelRequest, GetChannelResponse>(request, typeof(ChannelsIndex), cancellationToken);
    }
    
    public async Task<Channel?> GetChannelInformationAsync(string broadcasterId, CancellationToken cancellationToken = default)
    {
        var channels = await GetChannelInformationAsync(new GetChannelRequest(broadcasterId), cancellationToken);
        var channel = channels?.ChannelInformations.FirstOrDefault();

        if (channel is not null)
        {
            await Twitch.InjectDataAsync(channel, cancellationToken);
        }

        return channel;
    }
    
    public async Task<Channel?> GetChannelInformationAsync(User user, CancellationToken cancellationToken = default)
    {
        var channels = await GetChannelInformationAsync(new GetChannelRequest(user.Id), cancellationToken);
        var channel = channels?.ChannelInformations.FirstOrDefault();

        if (channel is not null)
        {
            await Twitch.InjectDataAsync(channel, cancellationToken);
        }

        return channel;
    }
    
    [ApiRules(RouteRules.RequiresOwner | RouteRules.RequiresModerator)]
    [ApiRoute("GET", "channels/followers", "moderator:read:followers")]
    [RequiresToken(TokenType.UserAccess)]
    public async Task<GetChannelFollowersResponse?> GetChannelFollowersAsync(GetChannelFollowersRequest request, CancellationToken cancellationToken = default)
    {
        return await Twitch.GetTwitchApiAsync<GetChannelFollowersRequest, GetChannelFollowersResponse>(request, typeof(ChannelsIndex), cancellationToken);
    }
    
    [ApiRules(RouteRules.RequiresOwner | RouteRules.RequiresModerator)]
    [ApiRoute("GET", "channels/followers", "moderator:read:followers")]
    [RequiresToken(TokenType.UserAccess)]
    public async Task<GetAllChannelFollowersResponse> GetAllChannelFollowersAsync(GetChannelFollowersRequest request, CancellationToken cancellationToken = default)
    {
        return await Twitch.GetTwitchApiAllAsync<GetChannelFollowersRequest, GetChannelFollowersResponse, GetAllChannelFollowersResponse>(request, typeof(ChannelsIndex), cancellationToken);
    }
    
    public async Task<List<Follower>> GetChannelFollowersAsync(string broadcasterId, CancellationToken cancellationToken = default)
    {
        var followers = await GetChannelFollowersAsync(new GetChannelFollowersRequest(broadcasterId), cancellationToken);
        return followers?.Followers ?? [];
    }
    
    public async IAsyncEnumerable<Follower> GetChannelFollowersAsync(Channel channel, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = null;
        do
        {
            var request = new GetChannelFollowersRequest(channel.BroadcasterId)
            {
                After = cursor
            };
            
            var followers = await GetChannelFollowersAsync(request, cancellationToken);
            
            if (followers is null)
            {
                yield break;
            }

            foreach (var follower in followers.Followers)
                yield return follower;
            
            cursor = followers.Pagination.Cursor;
        } while (!cancellationToken.IsCancellationRequested && !string.IsNullOrEmpty(cursor));
    }
    
    public async Task<List<Follower>> GetAllChannelFollowersAsync(string broadcasterId, CancellationToken cancellationToken = default)
    {
        var followers = await GetAllChannelFollowersAsync(new GetChannelFollowersRequest(broadcasterId), cancellationToken);
        return followers.Followers;
    }
    
    public Task<bool> IsOwnerAsync(string broadcasterId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Twitch.Me?.Channel?.BroadcasterId == broadcasterId);
    }
    
    public Task<bool> IsOwnerAsync(Channel channel, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Twitch.Me?.Channel?.BroadcasterId == channel.BroadcasterId);
    }
}