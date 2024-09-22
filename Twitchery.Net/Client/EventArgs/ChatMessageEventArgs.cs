namespace TwitcheryNet.Client.EventArgs;

public class ChatMessageEventArgs : EventBaseEventArgs
{
    public string Message { get; set; } = string.Empty;
}