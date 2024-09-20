using Microsoft.Extensions.DependencyInjection;
using TwitcheryNet.Attributes;
using TwitcheryNet.Models.Helix;
using TwitcheryNet.Models.Helix.Chat;
using TwitcheryNet.Models.Helix.Chat.Messages;
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
        GetAllChattersAsync(new GetChattersRequest(broadcasterId, moderatorId)).Result.Followers;
    
    [ApiRoute("POST", "chat/messages", "user:write:chat")]
    public async Task<SendChatMessageResponse?> SendChatMessageUserAsync(SendChatMessageRequestBody requestBody, CancellationToken cancellationToken = default)
    {
        return await Twitch.PostTwitchApiAsync<SendChatMessageRequestBody, SendChatMessageResponse>(requestBody, typeof(ChatIndex), cancellationToken);
    }
    
    [ApiRoute("POST", "chat/messages", "user:bot", "channel:bot")]
    public async Task<SendChatMessageResponse?> SendChatMessageAppAsync(SendChatMessageRequestBody requestBody, CancellationToken cancellationToken = default)
    {
        return await Twitch.PostTwitchApiAsync<SendChatMessageRequestBody, SendChatMessageResponse>(requestBody, typeof(ChatIndex), cancellationToken);
    }
    
    [ApiRoute("GET", "chat/chatters", "moderator:read:chatters")]
    public async Task<GetChattersResponse?> GetChattersAsync(GetChattersRequest request, CancellationToken cancellationToken = default)
    {
        return await Twitch.GetTwitchApiAsync<GetChattersRequest, GetChattersResponse>(request, typeof(ChatIndex), cancellationToken);
    }
    
    public async Task<GetAllChattersResponse> GetAllChattersAsync(GetChattersRequest request, CancellationToken cancellationToken = default)
    {
        return await Twitch.GetTwitchApiAllAsync<GetChattersRequest, GetChattersResponse, GetAllChattersResponse>(request, typeof(ChatIndex), cancellationToken);
    }
}