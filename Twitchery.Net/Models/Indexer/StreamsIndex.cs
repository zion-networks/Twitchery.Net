using Microsoft.Extensions.DependencyInjection;
using TwitcheryNet.Extensions.TwitchApi;
using TwitcheryNet.Services.Interfaces;
using Stream = TwitcheryNet.Models.Helix.Streams.Stream;

namespace TwitcheryNet.Models.Indexer;

public class StreamsIndex
{
    private ITwitchApiService Twitch { get; }
    
    [ActivatorUtilitiesConstructor]
    public StreamsIndex(ITwitchApiService api)
    {
        Twitch = api;
    }
    
    public Stream? this[string login]
    {
        get
        {
            var stream = Twitch.GetStreamsAsync(userLogins: [ login ]).Result;
            return stream?.Streams.FirstOrDefault();
        }
    }
    
    public Stream? this[uint id]
    {
        get
        {
            var stream = Twitch.GetStreamsAsync(userIds: [ id.ToString() ]).Result;
            return stream?.Streams.FirstOrDefault();
        }
    }
}