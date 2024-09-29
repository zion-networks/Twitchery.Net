using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.CharityCampaign;

public class ChannelCharityProgressHandler : INotification
{
    public string SubscriptionType => "channel.charity_campaign.progress";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelCharityProgressHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelCharityProgressHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}