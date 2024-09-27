namespace TwitcheryNet.Attributes;

[AttributeUsage(AttributeTargets.Event)]
public class EventSubAttribute : Attribute
{
    public string EventSubType { get; }
    public string EventSubVersion { get; }
    public string[] RequiredScopes { get; }
    
    public EventSubAttribute(string eventSubType, string eventSubVersion, params string[] requiredScopes)
    {
        EventSubType = eventSubType;
        EventSubVersion = eventSubVersion;
        RequiredScopes = requiredScopes;
    }
}