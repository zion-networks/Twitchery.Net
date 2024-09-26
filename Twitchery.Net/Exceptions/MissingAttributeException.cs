using System.Reflection;

namespace TwitcheryNet.Exceptions;

public class MissingAttributeException<T> : Exception where T : Attribute
{
    public MissingAttributeException(MemberInfo member) : base($"Missing attribute {typeof(T).Name} on {member.Name}")
    {
        
    }
}