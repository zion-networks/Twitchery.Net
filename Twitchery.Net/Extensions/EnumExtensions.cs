using System.Reflection;
using TwitcheryNet.Attributes;

namespace TwitcheryNet.Extensions;

public static class EnumExtensions
{
    public static string? GetValue(this Enum e)
    {
        var type = e.GetType();
        var member = type.GetMember(e.ToString()).FirstOrDefault();
        var attribute = member?.GetCustomAttribute<ValueAttribute>();
        return attribute?.Value;
    }
}