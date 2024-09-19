using Microsoft.Extensions.DependencyInjection;
using TwitcheryNet.Extensions.TwitchApi;
using TwitcheryNet.Models.Helix.Users;
using TwitcheryNet.Services.Interfaces;

namespace TwitcheryNet.Models.Indexer;

public sealed class UsersIndex
{
    private ITwitchApiService Twitch { get; }
    
    [ActivatorUtilitiesConstructor]
    public UsersIndex(ITwitchApiService api)
    {
        Twitch = api;
    }
    
    public User? this[string login]
    {
        get
        {
            var user = Twitch.GetUsersAsync(userLogins: [ login ]).Result;
            return user?.Users.FirstOrDefault();
        }
    }
    
    public User? this[uint id]
    {
        get
        {
            var user = Twitch.GetUsersAsync(userIds: [ id.ToString() ]).Result;
            return user?.Users.FirstOrDefault();
        }
    }
}