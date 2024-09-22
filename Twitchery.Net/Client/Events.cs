using TwitcheryNet.Attributes;

namespace TwitcheryNet.Client;

public class Events
{
    public enum Channel
    {
        [Value("channel.follow")] NewFollower,
        [Value("channel.subscribe")] NewSubscriber,
        [Value("channel.chat.message")] ChatMessage,
    }
}