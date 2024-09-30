namespace TwitcheryNet.Commands;

public struct CommandResult
{
    public bool Success { get; }
    public string Message { get; }
    
    public CommandResult(bool success, string message)
    {
        Success = success;
        Message = message;
    }
}