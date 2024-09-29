using Microsoft.Extensions.Logging;
using TwitcheryNet.Misc;

namespace TwitcheryNet.Net.EventSub.Handler.Channel.AdBreak;

public class ChannelAdBreakBeginHandler : INotification
{
    public string SubscriptionType => "channel.ad_break.begin";
    public string SubscriptionVersion => "1";
    
    private ILogger<ChannelAdBreakBeginHandler> Logger { get; } =
        LoggerFactory
            .Create(b => b.AddConsole())
            .CreateLogger<ChannelAdBreakBeginHandler>();
    
    public Task Handle(EventSubClient client, string json)
    {
        this.LogStub();
        
        return Task.CompletedTask;
    }
}