using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.CharityCampaign;

public class ChannelCharityStartHandler : INotification
{
    public string SubscriptionType => "channel.charity_campaign.start";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelCharityStartHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelCharityStartHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}