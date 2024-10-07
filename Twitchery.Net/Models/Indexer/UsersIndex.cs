using Microsoft.Extensions.DependencyInjection;
using TwitcheryNet.Attributes;
using TwitcheryNet.Caching;
using TwitcheryNet.Models.Helix.Users;
using TwitcheryNet.Services.Interfaces;

namespace TwitcheryNet.Models.Indexer;

public sealed class UsersIndex
{
    private ITwitchery Twitch { get; }
    
    private SmartCache<User> UsersCache { get; }
    
    [ActivatorUtilitiesConstructor]
    public UsersIndex(ITwitchery api)
    {
        Twitch = api;
        UsersCache = Twitch.SmartCachePool.GetOrCreateCache(GetUserByLoginAsync);
    }
    
    public CachedValue<User>? this[string login] => UsersCache[login];
    
    [ApiRoute("GET", "users")]
    [RequiresToken(TokenType.Both)]
    public async Task<GetUsersResponse?> GetUsersAsync(GetUsersRequest request, CancellationToken cancellationToken = default)
    {
        var users = await Twitch.GetTwitchApiAsync<GetUsersRequest, GetUsersResponse>(request, typeof(UsersIndex), cancellationToken);

        if (users is null)
        {
            return null;
        }

        foreach (var user in users.Users)
        {
            await Twitch.InjectDataAsync(user, cancellationToken);
        }
        
        return users;
    }
    
    public async Task<User?> GetUserByLoginAsync(string login, bool noInject, CancellationToken cancellationToken = default)
    {
        var users = await GetUsersAsync(new GetUsersRequest { Logins = [ login ] }, cancellationToken);
        var user = users?.Users.FirstOrDefault();
        
        if (noInject is not false && user is not null)
        {
            await Twitch.InjectDataAsync(user, cancellationToken);
        }

        return user;
    }
    
    public async Task<List<User>> GetUsersByLoginAsync(IEnumerable<string> logins, CancellationToken cancellationToken = default)
    {
        var user = await GetUsersAsync(new GetUsersRequest { Logins = logins.ToList() }, cancellationToken);
        var users = user?.Users ?? [];
        
        foreach (var u in users)
            await Twitch.InjectDataAsync(u, cancellationToken);
        
        return users;
    }
    
    public async Task<User?> GetUserByIdAsync(uint id, CancellationToken cancellationToken = default)
    {
        var users = await GetUsersAsync(new GetUsersRequest { Ids = [ id.ToString() ] }, cancellationToken);
        var user = users?.Users.FirstOrDefault();
        
        if (user is not null)
        {
            await Twitch.InjectDataAsync(user, cancellationToken);
        }

        return user;
    }
    
    public async Task<List<User>> GetUsersByIdAsync(IEnumerable<uint> ids, CancellationToken cancellationToken = default)
    {
        var user = await GetUsersAsync(new GetUsersRequest { Ids = ids.Select(id => id.ToString()).ToList() }, cancellationToken);
        var users = user?.Users ?? [];
        
        foreach (var u in users)
            await Twitch.InjectDataAsync(u, cancellationToken);
        
        return users;
    }
}