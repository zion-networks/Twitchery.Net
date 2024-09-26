namespace TwitcheryNet.Events;

public class ErrorOccuredArgs : EventArgs
{
    public Exception Exception { get; internal set; }
    public string Message { get; internal set; }
    
    public ErrorOccuredArgs(Exception exception, string? message = null)
    {
        Exception = exception;
        Message = message ?? string.Empty;
    }
}