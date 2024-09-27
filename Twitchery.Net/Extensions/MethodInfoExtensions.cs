using System.Reflection;

namespace TwitcheryNet.Extensions;

public static class MethodInfoExtensions
{
    public static bool HasCustomAttribute<T>(this MethodInfo m) where T : Attribute
    {
        return m.GetCustomAttribute<T>() != null;
    }
}