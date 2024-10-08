using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.CharityCampaign;

public class ChannelCharityDonationHandler : INotification
{
    public string SubscriptionType => "channel.charity_campaign.donate";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelCharityDonationHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelCharityDonationHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}