namespace TwitcheryNet.Caching.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class CachingAttribute : Attribute
{
    public int CacheDuration { get; set; }
    
    public CachingAttribute(int cacheDuration)
    {
        CacheDuration = cacheDuration;
    }
}