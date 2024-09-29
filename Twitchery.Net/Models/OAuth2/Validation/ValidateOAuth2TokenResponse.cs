using Newtonsoft.Json;

namespace TwitcheryNet.Models.OAuth2.Validation;

[JsonObject]
public class ValidateOAuth2TokenResponse
{
    [JsonProperty("client_id")]
    public string? ClientId { get; set; }
    
    [JsonProperty("login")]
    public string? Login { get; set; }
    
    [JsonProperty("scopes")]
    public List<string>? Scopes { get; set; }
    
    [JsonProperty("user_id")]
    public string? UserId { get; set; }
    
    [JsonProperty("expires_in")]
    public int? ExpiresIn { get; set; }
    
    [JsonProperty("status")]
    public string? Status { get; set; }
    
    [JsonProperty("message")]
    public string? Message { get; set; }
}