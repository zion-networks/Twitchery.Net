using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TwitcheryNet.Attributes;
using TwitcheryNet.Events;
using TwitcheryNet.Models.Helix.EventSub.Subscriptions;
using TwitcheryNet.Models.Indexer;
using TwitcheryNet.Net.EventSub;
using TwitcheryNet.Net.EventSub.EventArgs.Channel;
using TwitcheryNet.Net.EventSub.EventArgs.Channel.Chat;
using TwitcheryNet.Services.Interfaces;

namespace TwitcheryNet.Models.Helix.Channels;

[JsonObject]
public class Channel : IHasTwitchery, IConditional
{
    public ITwitchery? Twitch { get; set; }

    private ILogger<Channel> Logger { get; } = LoggerFactory
        .Create(x => x.AddConsole())
        .CreateLogger<Channel>();
    
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
    [InjectRouteData(typeof(ChannelsIndex), nameof(ChannelsIndex.IsOwnerAsync))]
    public bool IsOwner { get; set; }
    
    [JsonIgnore]
    [InjectRouteData(typeof(ModerationIndex), nameof(ModerationIndex.IsModeratorAsync))]
    public bool IsModerator { get; set; }
    
    [JsonIgnore]
    [InjectRouteData(typeof(ChannelsIndex), nameof(ChannelsIndex.GetChannelFollowersAsync))]
    public IAsyncEnumerable<Follower>? Followers { get; set; }
    
    [JsonIgnore]
    [InjectRouteData(typeof(ModerationIndex), nameof(ModerationIndex.GetAllModeratorsAsync))]
    public List<UserBase> Moderators { get; set; } = [];

    public EventSubCondition ToCondition()
    {
        return new EventSubCondition
        {
            UserId = Twitch?.Me?.Id,
            ModeratorUserId = Twitch?.Me?.Id,
            BroadcasterUserId = BroadcasterId
        };
    }

    [EventSub("channel.chat.message", "1", "chat:read")]
    public event AsyncEventHandler<ChannelChatMessageNotification>? ChatMessage
    {
        add => Twitch?.EventSubClient.RegisterEventSubAsync(this, nameof(ChatMessage), BroadcasterId, value ?? throw new ArgumentNullException(nameof(value))).Wait();
        remove => Twitch?.EventSubClient.UnregisterEventSubAsync(this, nameof(ChatMessage), BroadcasterId, value ?? throw new ArgumentNullException(nameof(value))).Wait();
    }
    
    [EventSub("channel.follow", "2", "moderator:read:followers")]
    public event AsyncEventHandler<ChannelFollowNotification>? Follow
    {
        add => Twitch?.EventSubClient.RegisterEventSubAsync(this, nameof(Follow), BroadcasterId, value ?? throw new ArgumentNullException(nameof(value))).Wait();
        remove => Twitch?.EventSubClient.UnregisterEventSubAsync(this, nameof(Follow), BroadcasterId, value ?? throw new ArgumentNullException(nameof(value))).Wait();
    }
    
    [EventSub("channel.subscribe", "1", "channel:read:subscriptions")]
    public event AsyncEventHandler<ChannelSubscribeNotification>? Subscribe
    {
        add => Twitch?.EventSubClient.RegisterEventSubAsync(this, nameof(Subscribe), BroadcasterId, value ?? throw new ArgumentNullException(nameof(value))).Wait();
        remove => Twitch?.EventSubClient.UnregisterEventSubAsync(this, nameof(Subscribe), BroadcasterId, value ?? throw new ArgumentNullException(nameof(value))).Wait();
    }
}