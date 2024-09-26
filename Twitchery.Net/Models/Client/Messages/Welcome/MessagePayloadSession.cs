using Newtonsoft.Json;

namespace TwitcheryNet.Models.Client.Messages.Welcome;

[JsonObject]
public class MessagePayloadSession
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonProperty("connected_at")]
    public DateTime ConnectedAt { get; set; }
    
    [JsonProperty("keepalive_timeout_seconds")]
    public int KeepAliveTimeoutSeconds { get; set; }
    
    [JsonProperty("reconnect_url")]
    public string? ReconnectUrl { get; set; }
}