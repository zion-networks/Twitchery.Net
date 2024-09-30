using TwitcheryNet.Net.EventSub.EventArgs.Channel.Chat;

namespace TwitcheryNet.Commands;

public class SimpleCommand : ICommand
{
    public string Name { get; }
    private Func<ChannelChatMessageNotification, Task<CommandResult>> AsyncHandler { get; }
    
    public SimpleCommand(string name, Func<ChannelChatMessageNotification, Task<CommandResult>> asyncHandler)
    {
        Name = name;
        AsyncHandler = asyncHandler;
    }
    
    public async Task<CommandResult> Execute(ChannelChatMessageNotification message)
    {
        return await AsyncHandler(message);
    }
}