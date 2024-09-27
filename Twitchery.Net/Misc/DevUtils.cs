using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using TwitcheryNet.Services.Implementations;

namespace TwitcheryNet.Misc;

public static class DevUtils
{
    private static ILogger<Twitchery> Logger { get; } = LoggerFactory
        .Create(x => x
            .AddConsole()
            .SetMinimumLevel(LogLevel.Debug))
        .CreateLogger<Twitchery>();
    
    public static void LogStub(this object o, [CallerMemberName] string? caller = null)
    {
        Logger.LogWarning("[TODO] Stub called: {ClassName}.{MethodName}", o.GetType().Name, caller);
    }
}