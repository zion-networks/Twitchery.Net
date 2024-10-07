using TwitcheryNet.Models.Helix.Channels;
using TwitcheryNet.Models.Helix.Polls;

namespace TwitcheryNet.Models.Extensions.Helix.Channels;

public static class ChannelExtensions
{
    public static async Task<Poll?> StartPollAsync(this Channel channel, string title, IEnumerable<string> choices, int duration = 300, CancellationToken cancellationToken = default)
    {
        if (channel.Twitch is null)
        {
            throw new InvalidOperationException("The channel does not have a Twitch instance.");
        }
        
        var poll = await channel.Twitch.Polls.CreatePollAsync(title, choices, channel, duration, cancellationToken);

        return poll?.Polls.FirstOrDefault();
    }
}