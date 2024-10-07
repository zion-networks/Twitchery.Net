using TwitcheryNet.Models.Helix.Channels;
using TwitcheryNet.Models.Helix.Polls;
using TwitcheryNet.Services.Interfaces;

namespace TwitcheryNet.Models.Extensions.Helix.Channels;

public static class ChannelExtensions
{
    public static async Task<Poll?> StartPollAsync(this Channel channel, string title, IEnumerable<string> choices, int duration = 300, CancellationToken cancellationToken = default)
    {
        var twitch = ((IHasTwitchery)channel).Twitch;
        
        if (twitch is null)
        {
            throw new InvalidOperationException("The channel does not have a Twitch instance.");
        }
        
        var poll = await twitch.Polls.CreatePollAsync(title, choices, channel, duration, cancellationToken);

        return poll?.Polls.FirstOrDefault();
    }
}