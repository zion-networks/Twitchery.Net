using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.UnbanRequest;

public class ChannelUnbanRequestCreateHandler : INotification
{
    public string SubscriptionType => "channel.unban_request.create";
    public string SubscriptionVersion => "1";
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        return Task.CompletedTask;
    }
}