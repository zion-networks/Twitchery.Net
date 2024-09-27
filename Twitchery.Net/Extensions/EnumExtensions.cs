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

    public static string? GetVersion(this Enum e)
    {
        var type = e.GetType();
        var member = type.GetMember(e.ToString()).FirstOrDefault();
        var attribute = member?.GetCustomAttribute<VersionAttribute>();
        return attribute?.Version;
    }
    
    public static T? FromValue<T>(string value) where T : Enum
    {
        var type = typeof(T);
        foreach (var field in type.GetFields())
        {
            var attribute = field.GetCustomAttribute<ValueAttribute>();
            if (attribute?.Value == value)
            {
                if (field.GetValue(null) is T result)
                {
                    return result;
                }
            }
        }

        return default;
    }
}