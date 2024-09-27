using TwitcheryNet.Models.Helix.Chat.Messages;
using TwitcheryNet.Net.EventSub.EventArgs.Channel;
using TwitcheryNet.Services.Interfaces;

namespace TwitcheryNet.Net.EventSub.EventArgs.Extensions.Channel;

public static class ChatMesageNotificationExtensions
{
    public static async Task ReplyAsync(this ChatMessageNotification msg, string reply, CancellationToken token = default)
    {
        if (msg is not IHasTwitchery tMsg)
        {
            return;
        }
        
        if (tMsg.Twitch?.Me?.Id is not null)
        {
            var senderId = tMsg.Twitch.Me.Id;
            
            var req = new SendChatMessageRequestBody(msg.BroadcasterUserId, senderId, reply)
            {
                ReplyParentMessageId = msg.MessageId
            };
            
            await tMsg.Twitch.Chat.SendChatMessageAppAsync(req, token);
        }
    }
}