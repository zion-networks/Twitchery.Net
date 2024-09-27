namespace TwitcheryNet.Misc;

public static class TaskUtils
{
    public static CancellationToken TimeoutToken(int seconds = 10)
    {
        return new CancellationTokenSource(TimeSpan.FromSeconds(seconds)).Token;
    }
}