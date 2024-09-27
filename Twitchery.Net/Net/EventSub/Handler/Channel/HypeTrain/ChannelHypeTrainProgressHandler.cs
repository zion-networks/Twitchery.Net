using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.HypeTrain;

public class ChannelHypeTrainProgressHandler : INotification
{
    public string SubscriptionType => "channel.hype_train.progress";
    public string SubscriptionVersion => "1";
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}