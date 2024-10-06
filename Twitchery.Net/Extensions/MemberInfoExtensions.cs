using System.Reflection;

namespace TwitcheryNet.Extensions;

public static class MemberInfoExtensions
{
    public static bool HasCustomAttribute<T>(this MemberInfo m) where T : Attribute
    {
        return m.GetCustomAttribute<T>() != null;
    }

    public static bool TryGetCustomAttribute<T>(this MemberInfo m, out T attribute) where T : Attribute
    {
        var a = m.GetCustomAttribute<T>();
        
        if (a != null)
        {
            attribute = a;
        }
        else
        {
            attribute = null!;
        }

        return a is not null;
    }
}