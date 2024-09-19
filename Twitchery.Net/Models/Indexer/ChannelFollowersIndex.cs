using Microsoft.Extensions.DependencyInjection;
using TwitcheryNet.Extensions.TwitchApi;
using TwitcheryNet.Models.Helix.Channels;
using TwitcheryNet.Services.Interfaces;

namespace TwitcheryNet.Models.Indexer;

public class ChannelFollowersIndex
{
    private ITwitchApiService Twitch { get; }
    
    [ActivatorUtilitiesConstructor]
    public ChannelFollowersIndex(ITwitchApiService api)
    {
        Twitch = api;
    }
    
    public List<Follower> this[string broadcasterId]
    {
        get
        {
            var followers = Twitch.GetAllChannelFollowersAsync(broadcasterId: broadcasterId).Result;
            return followers.Followers;
        }
    }
    
    public List<Follower> this[uint broadcasterId]
    {
        get
        {
            var followers = Twitch.GetAllChannelFollowersAsync(broadcasterId: broadcasterId.ToString()).Result;
            return followers.Followers;
        }
    }
}