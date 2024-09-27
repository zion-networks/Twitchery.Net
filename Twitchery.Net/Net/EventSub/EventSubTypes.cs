using TwitcheryNet.Attributes;

namespace TwitcheryNet.Net.EventSub;

public static class EventSubTypes
{
    public static class Channel
    {
        //[Value("channel.follow"), Version("2")] NewFollower,
        //[Value("channel.subscribe"), Version("1")] NewSubscriber,
        //[Value("channel.chat.message"), Version("1")] ChatMessage,
        
        public static readonly EventSubType ChatMessage = new("channel.chat.message", "1");
        public const string Test = "test";
    }
}