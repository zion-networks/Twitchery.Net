using TwitcheryNet.Models.Helix;

namespace TwitcheryNet.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class ApiRules : Attribute
{
    public RouteRules Rules { get; }
    
    public ApiRules(RouteRules rules)
    {
        Rules = rules;
    }
}