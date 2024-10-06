using Newtonsoft.Json;
using TwitcheryNet.Attributes;
using TwitcheryNet.Caching;
using TwitcheryNet.Caching.Attributes;
using TwitcheryNet.Json.Converter;
using TwitcheryNet.Misc;
using TwitcheryNet.Models.Helix.Channels;
using TwitcheryNet.Models.Indexer;

namespace TwitcheryNet.Models.Helix.Users;

[JsonObject]
[Caching(Seconds.Minute)]
public class User : ICachable
{
    [JsonProperty("id")]
    [CachingKey]
    public string Id { get; set; } = string.Empty;
    
    [JsonProperty("login")]
    public string Login { get; set; } = string.Empty;
    
    [JsonProperty("display_name")]
    public string DisplayName { get; set; } = string.Empty;
    
    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonProperty("broadcaster_type")]
    [JsonConverter(typeof(JsonBroadcasterTypeConverter))]
    public BroadcasterType BroadcasterType { get; set; } = BroadcasterType.Normal;
    
    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonProperty("profile_image_url")]
    public string ProfileImageUrl { get; set; } = string.Empty;
    
    [JsonProperty("offline_image_url")]
    public string OfflineImageUrl { get; set; } = string.Empty;
    
    [JsonProperty("view_count")]
    [Obsolete("This field has been deprecated. Any data in this field is not valid and should not be used.")]
    public int ViewCount { get; set; }
    
    [JsonProperty("email")]
    [NoCaching]
    public string Email { get; set; } = string.Empty;
    
    [JsonProperty("created_at")]
    [NoCaching]
    public DateTime CreatedAt { get; set; }
    
    [JsonIgnore]
    [CacheRetain]
    [InjectRouteData(typeof(ChannelsIndex), nameof(ChannelsIndex.GetChannelInformationAsync))]
    internal CachedValue<Channel>? CachedChannel { get; set; } // TODO: Replace with something like a 'CacheLink', so the actual value is bound to the cache
    
    [JsonIgnore]
    [NoCaching]
    public Channel? Channel => CachedChannel?.Value;

    public string GetCacheKey() => Id;
}