using TwitcheryNet.Attributes;
using TwitcheryNet.Models.Helix;

namespace TwitcheryNet.Models.Auth.Flow;

public class ClientCredentialsFlowRequest : IQueryParameters
{
    [QueryParameter("client_id", true)]
    public string ClientId { get; set; }
    
    [QueryParameter("client_secret", true)]
    public string ClientSecret { get; set; }
    
    [QueryParameter("grant_type", true)]
    public string GrantType => "client_credentials";
    
    public ClientCredentialsFlowRequest(string clientId, string clientSecret)
    {
        ClientId = clientId;
        ClientSecret = clientSecret;
    }
}