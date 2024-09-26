using System.Net;

namespace TwitcheryNet.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class ApiRoute : Attribute
{
    public string HttpMethod { get; }
    public string Path { get; }
    public string[] RequiredScopes { get; }
    public HttpStatusCode RequiredStatusCode { get; set; } = HttpStatusCode.OK;
    
    public ApiRoute(string httpMethod, string path, params string[] requiredScopes)
    {
        HttpMethod = httpMethod;
        Path = path;
        RequiredScopes = requiredScopes;
    }
}