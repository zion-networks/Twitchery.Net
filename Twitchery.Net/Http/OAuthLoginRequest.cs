using Newtonsoft.Json;

namespace TwitcheryNet.Http;

[JsonObject]
public class OAuthLoginRequest
{
    [JsonProperty("accessToken")]
    public string? AccessToken { get; set; }
    
    [JsonProperty("scope")]
    public string? Scope { get; set; }
    
    [JsonProperty("state")]
    public string? State { get; set; }
    
    [JsonProperty("tokenType")]
    public string? TokenType { get; set; }
}