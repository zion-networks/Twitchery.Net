using Microsoft.Extensions.DependencyInjection;
using TwitcheryNet.Attributes;
using TwitcheryNet.Models.Helix.Channels;
using TwitcheryNet.Models.Helix.Moderation;
using TwitcheryNet.Services.Interfaces;

namespace TwitcheryNet.Models.Indexer;

public class ModerationIndex
{
    private ITwitchery Twitch { get; }
    
    [ActivatorUtilitiesConstructor]
    public ModerationIndex(ITwitchery twitchery)
    {
        Twitch = twitchery;
    }
    
    [ApiRoute("GET", "moderation/channels", "user:read:moderated_channels")]
    public async Task<GetModeratedChannelsResponse?> GetModeratedChannelsAsync(GetModeratedChannelsRequest request, CancellationToken token = default)
    {
        var response = await Twitch.GetTwitchApiAsync<GetModeratedChannelsRequest, GetModeratedChannelsResponse>(request, typeof(ModerationIndex), token);
        
        return response;
    }
    
    [ApiRoute("GET", "moderation/channels", "user:read:moderated_channels")]
    public async Task<GetAllModeratedChannelsRequest> GetAllModeratedChannelsAsync(GetModeratedChannelsRequest request, CancellationToken token = default)
    {
        return await Twitch
            .GetTwitchApiAllAsync<GetModeratedChannelsRequest, GetModeratedChannelsResponse,
                GetAllModeratedChannelsRequest>(request, typeof(ModerationIndex), token);
    }
    
    public async Task<List<ModeratedChannel>> GetModeratedChannelsAsync(string userId, CancellationToken token = default)
    {
        var request = new GetModeratedChannelsRequest(userId);
        var response = await GetModeratedChannelsAsync(request, token);
        var channels = new List<ModeratedChannel>();
        
        if (response is not null)
        {
            channels.AddRange(response.Data);
        }

        return channels;
    }
    
    public async Task<List<ModeratedChannel>> GetAllModeratedChannelsAsync(string userId, CancellationToken token = default)
    {
        var allModeratedChannels = await GetAllModeratedChannelsAsync(new GetModeratedChannelsRequest(userId), token);
        
        return allModeratedChannels.Channels;
    }
    
    public async Task<bool> IsModeratorAsync(string userId, string channelId, CancellationToken token = default)
    {
        var channels = await GetAllModeratedChannelsAsync(userId, token);
        
        return channels.Any(x => x.BroadcasterId == channelId);
    }
    
    public async Task<bool> IsModeratorAsync(Channel channel, CancellationToken token = default)
    {
        var userId = Twitch.Me?.Id;

        if (userId is null)
        {
            return false;
        }
        
        var channels = await GetAllModeratedChannelsAsync(userId, token);
        
        return channels.Any(x => x.BroadcasterId == channel.BroadcasterId);
    }
}