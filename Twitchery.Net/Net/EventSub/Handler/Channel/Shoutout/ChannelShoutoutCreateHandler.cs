using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.Shoutout;

public class ChannelShoutoutCreateHandler : INotification
{
    public string SubscriptionType => "channel.shoutout.create";
    public string SubscriptionVersion => "1";
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}