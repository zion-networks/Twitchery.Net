namespace TwitcheryNet.Net.EventSub;

public class EventSubType
{
    public string Type { get; }
    public string Version { get; }
    
    public EventSubType(string type, string version)
    {
        Type = type;
        Version = version;
    }
    
    public override string ToString() => Type;
}