using System.Reflection;
using TwitcheryNet.Attributes;

namespace TwitcheryNet.Models.Helix;

public class Route
{
    public ApiRoute ApiRoute { get; }
    public ApiRules? ApiRules { get; }
    public MethodInfo CallerMethod { get; }
    public string Endpoint { get; }
    public string FullUrl => $"{Endpoint}{ApiRoute.Path}";
    
    public Route(string endpoint, ApiRoute apiRoute, MethodInfo callerMethod, ApiRules? apiRules = null)
    {
        Endpoint = endpoint;
        ApiRoute = apiRoute;
        ApiRules = apiRules;
        CallerMethod = callerMethod;
    }
}