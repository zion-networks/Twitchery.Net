namespace TwitcheryNet.Net.EventSub.Handler.Channel;

public class ChatMessageHandler : INotification
{
    public string SubscriptionType => "channel.chat.message";
    
    public Task Handle(EventSubClient client, string json)
    {
        throw new NotImplementedException();
    }
}