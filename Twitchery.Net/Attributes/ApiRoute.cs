using System.Net;

namespace TwitcheryNet.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class ApiRoute : Attribute
{
    public string HttpMethod { get; }
    public string Route { get; }
    public string[] RequiredScopes { get; }
    public HttpStatusCode RequiredStatusCode { get; set; } = HttpStatusCode.OK;
    
    public ApiRoute(string httpMethod, string route, params string[] requiredScopes)
    {
        HttpMethod = httpMethod;
        Route = route;
        RequiredScopes = requiredScopes;
    }
}