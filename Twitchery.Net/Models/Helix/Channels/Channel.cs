using Newtonsoft.Json;
using TwitcheryNet.Attributes;
using TwitcheryNet.Client;
using TwitcheryNet.Client.EventArgs;
using TwitcheryNet.Exceptions;
using TwitcheryNet.Models.Indexer;
using TwitcheryNet.Services.Implementations;
using TwitcheryNet.Services.Interfaces;

namespace TwitcheryNet.Models.Helix.Channels;

[JsonObject]
public class Channel : IHasTwitchery
{
    public ITwitchery? Twitch { get; set; }
    
    [JsonProperty("broadcaster_id")]
    public string BroadcasterId { get; set; } = string.Empty;
    
    [JsonProperty("broadcaster_login")]
    public string BroadcasterLogin { get; set; } = string.Empty;
    
    [JsonProperty("broadcaster_name")]
    public string BroadcasterName { get; set; } = string.Empty;
    
    [JsonProperty("broadcaster_language")]
    public string BroadcasterLanguage { get; set; } = string.Empty;
    
    [JsonProperty("game_name")]
    public string GameName { get; set; } = string.Empty;
    
    [JsonProperty("game_id")]
    public string GameId { get; set; } = string.Empty;
    
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonProperty("delay")]
    public uint Delay { get; set; }
    
    [JsonProperty("tags")]
    public List<string> Tags { get; set; } = [];
    
    [JsonProperty("content_classification_labels")]
    public List<string> ContentClassificationLabels { get; set; } = [];
    
    [JsonProperty("is_branded_content")]
    public bool IsBrandedContent { get; set; }
    
    [JsonIgnore]
    [InjectRouteData(typeof(ChannelsIndex), nameof(ChannelsIndex.GetChannelFollowersAsync))]
    public IAsyncEnumerable<Follower>? Followers { get; set; }
    
    public event EventHandler<NewFollowerEventArgs>? NewFollower
    {
        add
        {
            if (value is null)
                return;
            
            if (Twitch is Twitchery twitchery)
            {
                MissingTwitchScopeException.ThrowIfMissing(twitchery.ClientScopes, "moderator:read:followers");
                
                twitchery.WebSocketClient.SubscribeEventAsync(this, Events.Channel.NewFollower, value).Wait();
            }
        }
        remove
        {
            
        }
    }
    
    public event EventHandler<ChatMessageEventArgs>? ChatMessage
    {
        add
        {
            if (value is null)
                return;
            
            if (Twitch is Twitchery twitchery)
            {
                MissingTwitchScopeException.ThrowIfMissing(twitchery.ClientScopes, "chat:read");
                
                twitchery.WebSocketClient.SubscribeEventAsync(this, Events.Channel.ChatMessage, value).Wait();
            }
        }
        remove
        {
            
        }
    } 
}