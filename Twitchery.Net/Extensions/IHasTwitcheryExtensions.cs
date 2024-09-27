using Microsoft.Extensions.Logging;
using TwitcheryNet.Services.Interfaces;

namespace TwitcheryNet.Extensions;

public static class HasTwitcheryExtensions
{
    public static void InjectTwitchery<T>(this T target, ITwitchery twitchery)
        where T : class
    {
        if (target is IHasTwitchery hasTwitchery)
        {
            hasTwitchery.Twitch = twitchery;
        }
    }
}