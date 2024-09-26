namespace TwitcheryNet.Net.EventSub;

public interface INotification
{
    public string SubscriptionType { get; }

    public Task Handle(EventSubClient client, string json);
}