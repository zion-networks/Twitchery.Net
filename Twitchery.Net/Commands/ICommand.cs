using TwitcheryNet.Net.EventSub.EventArgs.Channel.Chat;

namespace TwitcheryNet.Commands;

public interface ICommand
{
    public string Name { get; }
    
    public Task<CommandResult> Execute(ChannelChatMessageNotification message);
}