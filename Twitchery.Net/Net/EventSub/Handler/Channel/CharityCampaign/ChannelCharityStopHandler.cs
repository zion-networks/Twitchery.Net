using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.CharityCampaign;

public class ChannelCharityStopHandler : INotification
{
    public string SubscriptionType => "channel.charity_campaign.stop";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelCharityStopHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelCharityStopHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}