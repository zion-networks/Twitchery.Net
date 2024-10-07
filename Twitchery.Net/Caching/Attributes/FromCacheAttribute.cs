namespace TwitcheryNet.Caching.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class FromCacheAttribute : Attribute
{
    public string KeyPropertyName { get; }
    
    public FromCacheAttribute(string keyPropertyName)
    {
        KeyPropertyName = keyPropertyName;
    }
}