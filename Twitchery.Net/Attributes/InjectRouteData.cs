namespace TwitcheryNet.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class InjectRouteData : Attribute
{
    public Type SourceType { get; }
    public string SourceMethodName { get; }
    
    public InjectRouteData(Type sourceType, string sourceMethodName)
    {
        SourceType = sourceType;
        SourceMethodName = sourceMethodName;
    }
}