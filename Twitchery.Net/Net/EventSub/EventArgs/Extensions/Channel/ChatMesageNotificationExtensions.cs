using TwitcheryNet.Models.Helix.Chat.Messages;
using TwitcheryNet.Net.EventSub.EventArgs.Channel;
using TwitcheryNet.Net.EventSub.EventArgs.Channel.Chat;
using TwitcheryNet.Services.Interfaces;

namespace TwitcheryNet.Net.EventSub.EventArgs.Extensions.Channel;

public static class ChatMesageNotificationExtensions
{
    public static async Task<bool> ReplyAsync(this ChannelChatMessageNotification msg, string reply, CancellationToken token = default)
    {
        if (msg is not IHasTwitchery tMsg)
        {
            return false;
        }
        
        if (tMsg.Twitch?.Me?.Id is not null)
        {
            var senderId = tMsg.Twitch.Me.Id;
            
            var req = new SendChatMessageRequestBody(msg.BroadcasterUserId, senderId, reply)
            {
                ReplyParentMessageId = msg.MessageId
            };
            
            await tMsg.Twitch.Chat.SendChatMessageUserAsync(req, token);
            
            return true;
        }
        
        return false;
    }
}