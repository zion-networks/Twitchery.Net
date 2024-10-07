using System.Net;
using Microsoft.Extensions.DependencyInjection;
using TwitcheryNet.Attributes;
using TwitcheryNet.Models.Helix;
using TwitcheryNet.Models.Helix.Channels;
using TwitcheryNet.Models.Helix.Chat;
using TwitcheryNet.Models.Helix.Chat.Messages;
using TwitcheryNet.Models.Helix.Chat.Shoutouts;
using TwitcheryNet.Services.Implementations;

namespace TwitcheryNet.Models.Indexer;

public class ChatIndex
{
    private Twitchery Twitch { get; }
    
    [ActivatorUtilitiesConstructor]
    public ChatIndex(Twitchery api)
    {
        Twitch = api;
    }

    public List<UserBase> this[string broadcasterId, string moderatorId] =>
        GetAllChattersAsync(new GetChattersRequest(broadcasterId, moderatorId)).Result.Chatters;
    
    public List<UserBase> this[string broadcasterId] =>
        GetAllChattersAsync(new GetChattersRequest(broadcasterId, broadcasterId)).Result.Chatters;
    
    [ApiRoute("POST", "chat/messages", "user:write:chat")]
    [RequiresToken(TokenType.UserAccess)]
    public async Task<SendChatMessageResponse?> SendChatMessageUserAsync(SendChatMessageRequestBody requestBody, CancellationToken cancellationToken = default)
    {
        return await Twitch.PostTwitchApiAsync<SendChatMessageRequestBody, SendChatMessageResponse>(requestBody, typeof(ChatIndex), cancellationToken);
    }
    
    [ApiRoute("POST", "chat/messages", "user:bot", "channel:bot")]
    [RequiresToken(TokenType.AppAccess)]
    public async Task<SendChatMessageResponse?> SendChatMessageAppAsync(SendChatMessageRequestBody requestBody, CancellationToken cancellationToken = default)
    {
        return await Twitch.PostTwitchApiAsync<SendChatMessageRequestBody, SendChatMessageResponse>(requestBody, typeof(ChatIndex), cancellationToken);
    }
    
    [ApiRoute("GET", "chat/chatters", "moderator:read:chatters")]
    [RequiresToken(TokenType.UserAccess)]
    public async Task<GetChattersResponse?> GetChattersAsync(GetChattersRequest request, CancellationToken cancellationToken = default)
    {
        return await Twitch.GetTwitchApiAsync<GetChattersRequest, GetChattersResponse>(request, typeof(ChatIndex), cancellationToken);
    }
    
    [ApiRoute("GET", "chat/chatters", "moderator:read:chatters")]
    [RequiresToken(TokenType.UserAccess)]
    public async Task<GetAllChattersResponse> GetAllChattersAsync(GetChattersRequest request, CancellationToken cancellationToken = default)
    {
        return await Twitch.GetTwitchApiAllAsync<GetChattersRequest, GetChattersResponse, GetAllChattersResponse>(request, typeof(ChatIndex), cancellationToken);
    }
    
    public async Task<List<UserBase>> GetAllChattersAsync(Channel channel, CancellationToken cancellationToken = default)
    {
        var request = new GetChattersRequest(channel.BroadcasterId, channel.BroadcasterId);
        var result = await Twitch.GetTwitchApiAllAsync<GetChattersRequest, GetChattersResponse, GetAllChattersResponse>(request, typeof(ChatIndex), cancellationToken);
        
        return result.Chatters;
    }
    
    [ApiRoute("POST", "chat/shoutouts", "moderator:manage:shoutouts", RequiredStatusCode = HttpStatusCode.NoContent)]
    [RequiresToken(TokenType.UserAccess)]
    public async Task SendShoutoutAsync(SendShoutoutRequest request, CancellationToken cancellationToken = default)
    {
        await Twitch.PostTwitchApiAsync(request, typeof(ChatIndex), cancellationToken);
    }
    
    public async Task SendShoutoutAsync(string fromBroadcasterId, string toBroadcasterId, string moderatorId, CancellationToken cancellationToken = default)
    {
        var request = new SendShoutoutRequest(fromBroadcasterId, toBroadcasterId, moderatorId);
        
        await Twitch.PostTwitchApiAsync(request, typeof(ChatIndex), cancellationToken);
    }
}