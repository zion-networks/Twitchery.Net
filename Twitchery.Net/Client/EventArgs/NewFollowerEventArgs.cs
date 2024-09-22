using TwitcheryNet.Models.Helix.Channels;

namespace TwitcheryNet.Client.EventArgs;

public class NewFollowerEventArgs : EventBaseEventArgs
{
    public Follower Follower { get; }
}