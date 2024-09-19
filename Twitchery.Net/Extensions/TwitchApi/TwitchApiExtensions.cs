using TwitcheryNet.Attributes;
using TwitcheryNet.Models.Helix.Streams;
using TwitcheryNet.Models.Helix.Users;
using TwitcheryNet.Services.Interfaces;

namespace TwitcheryNet.Extensions.TwitchApi;

public static class TwitchApiExtensions
{
    [ApiRoute("GET", "streams")]
    public static async Task<GetStreamsResponse?> GetStreamsAsync(
        this ITwitchApiService service,
        List<string>? userIds = null,
        List<string>? userLogins = null,
        List<string>? gameIds = null,
        string? type = null,
        List<string>? languages = null,
        int? first = null,
        string? before = null,
        string? after = null,
        CancellationToken cancellationToken = default)
    {
        var request = new GetStreamsRequest
        {
            UserId = userIds,
            UserLogin = userLogins,
            GameId = gameIds,
            Type = type,
            Language = languages,
            First = first,
            Before = before,
            After = after
        };
        
        return await service.GetTwitchApiAsync<GetStreamsRequest, GetStreamsResponse>(request, typeof(TwitchApiExtensions), cancellationToken);
    }
    
    [ApiRoute("GET", "users")]
    public static async Task<GetUsersResponse?> GetUsersAsync(
        this ITwitchApiService service,
        List<string>? userIds = null,
        List<string>? userLogins = null,
        CancellationToken cancellationToken = default)
    {
        var request = new GetUsersRequest
        {
            Id = userIds,
            Login = userLogins
        };
        
        return await service.GetTwitchApiAsync<GetUsersRequest, GetUsersResponse>(request, typeof(TwitchApiExtensions), cancellationToken);
    }
}