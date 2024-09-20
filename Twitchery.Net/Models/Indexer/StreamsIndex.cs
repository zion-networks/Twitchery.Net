using Microsoft.Extensions.DependencyInjection;
using TwitcheryNet.Attributes;
using TwitcheryNet.Models.Helix.Streams;
using TwitcheryNet.Services.Interfaces;
using Stream = TwitcheryNet.Models.Helix.Streams.Stream;

namespace TwitcheryNet.Models.Indexer;

public class StreamsIndex
{
    private ITwitchery Twitch { get; }
    
    [ActivatorUtilitiesConstructor]
    public StreamsIndex(ITwitchery api)
    {
        Twitch = api;
    }
    
    public Stream? this[string login] => GetStreamByLoginAsync(login).Result;

    public Stream? this[uint id] => GetStreamByIdAsync(id).Result;

    [ApiRoute("GET", "streams")]
    public async Task<GetStreamsResponse?> GetStreamsAsync(GetStreamsRequest request, CancellationToken cancellationToken = default)
    {
        return await Twitch.GetTwitchApiAsync<GetStreamsRequest, GetStreamsResponse>(request, typeof(StreamsIndex), cancellationToken);
    }
    
    public async Task<Stream?> GetStreamByLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        var stream = await GetStreamsAsync(new GetStreamsRequest { UserLogins = [ login ]}, cancellationToken);
        return stream?.Streams.FirstOrDefault();
    }
    
    public async Task<List<Stream>> GetStreamsByLoginAsync(IEnumerable<string> logins, CancellationToken cancellationToken = default)
    {
        var stream = await GetStreamsAsync(new GetStreamsRequest { UserLogins = logins.ToList() }, cancellationToken);
        return stream?.Streams ?? [];
    }
    
    public async Task<Stream?> GetStreamByIdAsync(uint id, CancellationToken cancellationToken = default)
    {
        var stream = await GetStreamsAsync(new GetStreamsRequest { UserIds = [ id.ToString() ] }, cancellationToken);
        return stream?.Streams.FirstOrDefault();
    }
    
    public async Task<List<Stream>> GetStreamsByIdAsync(IEnumerable<uint> ids, CancellationToken cancellationToken = default)
    {
        var stream = await GetStreamsAsync(new GetStreamsRequest { UserIds = ids.Select(id => id.ToString()).ToList() }, cancellationToken);
        return stream?.Streams ?? [];
    }
    
    public async Task<Stream?> GetStreamByGameIdAsync(uint gameId, CancellationToken cancellationToken = default)
    {
        var stream = await GetStreamsAsync(new GetStreamsRequest { GameIds = [ gameId.ToString() ] }, cancellationToken);
        return stream?.Streams.FirstOrDefault();
    }
    
    public async Task<List<Stream>> GetStreamsByGameIdAsync(IEnumerable<uint> gameIds, CancellationToken cancellationToken = default)
    {
        var stream = await GetStreamsAsync(new GetStreamsRequest { GameIds = gameIds.Select(id => id.ToString()).ToList() }, cancellationToken);
        return stream?.Streams ?? [];
    }
}