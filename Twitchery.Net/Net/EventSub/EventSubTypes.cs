using TwitcheryNet.Attributes;

namespace TwitcheryNet.Net.EventSub;

public class EventSubTypes
{
    public enum Channel
    {
        [Value("channel.follow"), Version("2")] NewFollower,
        [Value("channel.subscribe"), Version("1")] NewSubscriber,
        [Value("channel.chat.message"), Version("1")] ChatMessage,
    }
}