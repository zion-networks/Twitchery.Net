namespace TwitcheryNet.Exceptions;

public class ApiException : Exception
{
    public override string Message { get; }
    
    public ApiException(string message) : base(message)
    {
        Message = message;
    }
}