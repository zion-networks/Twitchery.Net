namespace TwitcheryNet.Events;

public class DataReceivedArgs : EventArgs
{
    public string Message { get; internal set; }
}