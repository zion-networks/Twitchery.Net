namespace TwitcheryNet.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class VersionAttribute : Attribute
{
    public string Version { get; }
    
    public VersionAttribute(string version)
    {
        Version = version;
    }
}