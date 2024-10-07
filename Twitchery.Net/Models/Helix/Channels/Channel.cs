using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TwitcheryNet.Attributes;
using TwitcheryNet.Caching;
using TwitcheryNet.Caching.Attributes;
using TwitcheryNet.Events;
using TwitcheryNet.Misc;
using TwitcheryNet.Models.Helix.EventSub.Subscriptions;
using TwitcheryNet.Models.Indexer;
using TwitcheryNet.Net.EventSub;
using TwitcheryNet.Net.EventSub.EventArgs.Channel;
using TwitcheryNet.Net.EventSub.EventArgs.Channel.Chat;
using TwitcheryNet.Services.Interfaces;

namespace TwitcheryNet.Models.Helix.Channels;

[JsonObject]
[Caching(Seconds.Minutes5)]
public class Channel : IHasTwitchery, IConditional, ICachable
{
    [NoCaching]
    ITwitchery? IHasTwitchery.Twitch { get; set; }

    [NoCaching]
    private ILogger<Channel> Logger { get; } = LoggerFactory
        .Create(x => x.AddConsole())
        .CreateLogger<Channel>();
    
    [JsonProperty("broadcaster_id")]
    [CachingKey]
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
    [NoCaching]
    public uint Delay { get; set; }
    
    [JsonProperty("tags")]
    public List<string> Tags { get; set; } = [];
    
    [JsonProperty("content_classification_labels")]
    public List<string> ContentClassificationLabels { get; set; } = [];
    
    [JsonProperty("is_branded_content")]
    public bool IsBrandedContent { get; set; }
    
    [JsonIgnore]
    public bool IsOwner => ((IHasTwitchery)this).Twitch?.Channels.IsOwnerAsync(this).Result ?? false;

    [JsonIgnore]
    public bool IsModerator => ((IHasTwitchery)this).Twitch?.Moderation.IsModeratorAsync(this).Result ?? false;

    // TODO: Should be cached as: BroadcasterId => List<Follower> with a lifespan of less or equal to 5 minutes
    [JsonIgnore]
    [CacheRetain]
    [InjectRouteData(typeof(ChannelsIndex), nameof(ChannelsIndex.GetAllChannelFollowersAsync))]
    public List<Follower> Followers { get; set; } = [];

    // TODO: Should be cached as: BroadcasterId => List<Chatter> with a lifespan of less or equal to 5 minutes
    [JsonIgnore]
    [CacheRetain]
    [InjectRouteData(typeof(ChatIndex), nameof(ChatIndex.GetAllChattersAsync))]
    public List<UserBase> Chatters { get; set; } = [];

    public EventSubCondition ToCondition()
    {
        return new EventSubCondition
        {
            UserId = ((IHasTwitchery)this).Twitch?.Me?.Id,
            ModeratorUserId = ((IHasTwitchery)this).Twitch?.Me?.Id,
            BroadcasterUserId = BroadcasterId
        };
    }

    [EventSub("channel.chat.message", "1", "chat:read")]
    public event AsyncEventHandler<ChannelChatMessageNotification>? ChatMessage
    {
        add => ((IHasTwitchery)this).Twitch?.EventSubClient.RegisterEventSubAsync(this, nameof(ChatMessage), BroadcasterId, value ?? throw new ArgumentNullException(nameof(value))).Wait();
        remove => ((IHasTwitchery)this).Twitch?.EventSubClient.UnregisterEventSubAsync(this, nameof(ChatMessage), BroadcasterId, value ?? throw new ArgumentNullException(nameof(value))).Wait();
    }
    
    [EventSub("channel.follow", "2", "moderator:read:followers")]
    public event AsyncEventHandler<ChannelFollowNotification>? Follow
    {
        add => ((IHasTwitchery)this).Twitch?.EventSubClient.RegisterEventSubAsync(this, nameof(Follow), BroadcasterId, value ?? throw new ArgumentNullException(nameof(value))).Wait();
        remove => ((IHasTwitchery)this).Twitch?.EventSubClient.UnregisterEventSubAsync(this, nameof(Follow), BroadcasterId, value ?? throw new ArgumentNullException(nameof(value))).Wait();
    }
    
    [EventSub("channel.subscribe", "1", "channel:read:subscriptions")]
    public event AsyncEventHandler<ChannelSubscribeNotification>? Subscribe
    {
        add => ((IHasTwitchery)this).Twitch?.EventSubClient.RegisterEventSubAsync(this, nameof(Subscribe), BroadcasterId, value ?? throw new ArgumentNullException(nameof(value))).Wait();
        remove => ((IHasTwitchery)this).Twitch?.EventSubClient.UnregisterEventSubAsync(this, nameof(Subscribe), BroadcasterId, value ?? throw new ArgumentNullException(nameof(value))).Wait();
    }

    public string GetCacheKey() => BroadcasterId;
}