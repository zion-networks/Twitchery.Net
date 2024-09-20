using TwitcheryNet.Attributes;
using TwitcheryNet.Models.Helix.Chat;
using TwitcheryNet.Models.Helix.Chat.Messages;
using TwitcheryNet.Services.Interfaces;

namespace TwitcheryNet.Extensions.TwitchApi;

public static class TwitchChatApiExtensions
{
    [ApiRoute("POST", "chat/messages", "user:write:chat")]
    public static async Task<SendChatMessageResponse?> SendChatMessageUserAsync(this ITwitchery service, string broadcasterId, string senderId, string message, string? replyParentMessageId = null, CancellationToken cancellationToken = default)
    {
        var requestBody = new SendChatMessageRequestBody(broadcasterId, senderId, message)
        {
            ReplyParentMessageId = replyParentMessageId
        };
        
        return await service.PostTwitchApiAsync<SendChatMessageRequestBody, SendChatMessageResponse>(requestBody, typeof(TwitchChatApiExtensions), cancellationToken);
    }
    
    [ApiRoute("POST", "chat/messages", "user:bot", "channel:bot")]
    public static async Task<SendChatMessageResponse?> SendChatMessageAppAsync(this ITwitchery service, string broadcasterId, string senderId, string message, string? replyParentMessageId = null, CancellationToken cancellationToken = default)
    {
        var requestBody = new SendChatMessageRequestBody(broadcasterId, senderId, message)
        {
            ReplyParentMessageId = replyParentMessageId
        };
        
        return await service.PostTwitchApiAsync<SendChatMessageRequestBody, SendChatMessageResponse>(requestBody, typeof(TwitchChatApiExtensions), cancellationToken);
    }
    
    [ApiRoute("GET", "chat/chatters", "moderator:read:chatters")]
    public static async Task<GetChattersResponse?> GetChattersAsync(this ITwitchery service, string broadcasterId, string moderatorId, int? first = null, string? after = null, CancellationToken cancellationToken = default)
    {
        var request = new GetChattersRequest(broadcasterId, moderatorId)
        {
            First = first,
            After = after
        };
        
        return await service.GetTwitchApiAsync<GetChattersRequest, GetChattersResponse>(request, typeof(TwitchChatApiExtensions), cancellationToken);
    }
}