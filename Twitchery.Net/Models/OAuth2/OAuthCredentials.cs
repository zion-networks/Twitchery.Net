using TwitcheryNet.Attributes;

namespace TwitcheryNet.Models.OAuth2;

public class OAuthCredentials
{
    public TokenType Type { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string AccessToken { get; set; }
    public string? RedirectUri { get; set; }
    public string[] Scopes { get; set; } = [];
    
    public OAuthCredentials(string clientId, string accessToken)
    {
        Type = TokenType.UserAccess;
        ClientId = clientId;
        AccessToken = accessToken;
        ClientSecret = string.Empty;
    }
    
    public OAuthCredentials(string clientId, string clientSecret, string accessToken)
    {
        Type = TokenType.AppAccess;
        ClientId = clientId;
        ClientSecret = clientSecret;
        AccessToken = accessToken;
    }
}