# How to contribute to Twitchery.Net

## Getting started

1. Clone the Twitchery.Net project with `git clone https://github.com/zion-networks/Twitchery.Net.git`
2. Open up the project with your favourite IDE

## Adding new EventSub handlers

1. Locate the directory `Twitchery.Net/Twitchery.Net/Net/EventSub`
2. The subdirectory `EventArgs` contains all the event arguments for the EventSub handlers
3. The subdirectory `Handler` contains all the EventSub handlers

The list of available Twitch EventSub topics can be found [here](https://dev.twitch.tv/docs/eventsub/eventsub-subscription-types/). 
Each topic requires a handler to be implemented in the `Handler` subdirectory and an event arguments class in the `EventArgs` subdirectory. 
To make contributing easier, all handlers are already created as stubs in the `Handler` subdirectory.

Directory structure for handlers:
- Example 1: `automod.message.hold` becomes `Net/EventSub/Handler/Automod/Message/AutomodMessageHoldHandler.cs`
- Example 2: `channel.follow` becomes `Net/EventSub/Handler/Channel/ChannelFollowHandler.cs`
- Example 3: `channel.channel_points_automatic_reward_redemption.add` becomes `Net/EventSub/Handler/Channel/ChannelPointsAutomaticRewardRedemption/ChannelPointsAutomaticRewardRedemptionHandler.cs`

Directory structure for event arguments:
- Example 1: `channel.chat.message` becomes `Net/EventSub/EventArgs/Channel/Chat/ChannelChatMessageNotification.cs`
- Example 2: `channel.follow` becomes `Net/EventSub/EventArgs/Channel/ChannelFollowNotification.cs`
- Example 3: `channel.subscribe` becomes `Net/EventSub/EventArgs/Channel/ChannelSubscribeNotification.cs`

### EventArgs classes

The EventArgs classes are used to deserialize the JSON payload from Twitch into a C# object. Example for `channel.follow`:

Documentation:
- [Overview](https://dev.twitch.tv/docs/eventsub/eventsub-subscription-types/#channelfollow)
- [Event Data](https://dev.twitch.tv/docs/eventsub/eventsub-reference/#channel-follow-event)

For the implementation only the Event Data is relevant. Everything else is already handles internally.

```json
{
  "user_id": "1234",
  "user_login": "cool_user",
  "user_name": "Cool_User",
  "broadcaster_user_id": "1337",
  "broadcaster_user_login": "cooler_user",
  "broadcaster_user_name": "Cooler_User",
  "followed_at": "2020-07-15T18:16:11.17106713Z"
}
```

becomes

```csharp
using Newtonsoft.Json;
using TwitcheryNet.Services.Interfaces;

// Make sure the namespace matches the directory structure
namespace TwitcheryNet.Net.EventSub.EventArgs.Channel;

[JsonObject] // Required for deserialization
public class ChannelFollowNotification : IHasTwitchery // 
{
    [JsonIgnore]
    public ITwitchery? Twitch { get; set; }
    
    [JsonProperty("user_id")]
    public string UserId { get; set; } = string.Empty;
    
    [JsonProperty("user_login")]
    public string UserLogin { get; set; } = string.Empty;
    
    [JsonProperty("user_name")]
    public string UserName { get; set; } = string.Empty;
    
    [JsonProperty("broadcaster_user_id")]
    public string BroadcasterUserId { get; set; } = string.Empty;
    
    [JsonProperty("broadcaster_user_login")]
    public string BroadcasterUserLogin { get; set; } = string.Empty;
    
    [JsonProperty("broadcaster_user_name")]
    public string BroadcasterUserName { get; set; } = string.Empty;
    
    [JsonProperty("followed_at")]
    public string FollowedAt { get; set; } = string.Empty;
}
```

Finally, the handler needs to be added to the respective Indexer. These can be found at `Models/Helix/Indexer`. 
Most of the handlers are related to a Channel, so the `ChannelIndexer` is the most common one.

Here's an example implementation for the `channel.follow` event:

```csharp
// ...

[EventSub("channel.follow", "2", "moderator:read:followers")]
public event AsyncEventHandler<ChannelFollowNotification>? Follow
{
    add => Twitch?.EventSubClient.RegisterEventSubAsync(this, nameof(Follow), value ?? throw new ArgumentNullException(nameof(value))).Wait();
    remove => Twitch?.EventSubClient.UnregisterEventSubAsync(this, nameof(Follow), value ?? throw new ArgumentNullException(nameof(value))).Wait();
}

// ...
```

The `EventSub` attribute requires the following parameters:
- The EventSub topic to listen to
- The version of the EventSub topic
- The required scope(s) to listen to the EventSub topic, if any