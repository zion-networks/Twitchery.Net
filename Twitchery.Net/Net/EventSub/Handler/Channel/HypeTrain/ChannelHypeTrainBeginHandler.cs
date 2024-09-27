using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.HypeTrain;

public class ChannelHypeTrainBeginHandler : INotification
{
    public string SubscriptionType => "channel.hype_train.begin";
    public string SubscriptionVersion => "1";
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}