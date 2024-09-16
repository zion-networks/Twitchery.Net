using TwitcheryNet.Http;
using TwitcheryNet.Services.Implementation;
using static TwitcheryNet.Tests.Constants;

namespace TwitcheryNet.Tests.Services;

public class Tests
{
    private TwitchApiService? _twitchApiService;
    
    [SetUp]
    public void Setup()
    {
        AsyncHttpClient.OnResponse = (response, body) =>
        {
            TestContext.Out.WriteLineAsync($"Response: [{response.StatusCode}] {body}");
        };
        
        _twitchApiService ??= new TwitchApiService
        {
            ClientId = ClientId
        };

        if (string.IsNullOrEmpty(_twitchApiService.AccessToken))
        {
            _twitchApiService.StartImplicitAuthenticationAsync("http://localhost:9999", [
                "bits:read",
                "chat:read",
                "channel:read:subscriptions",
                "channel:read:redemptions",
                "channel:read:hype_train",
                "channel:read:polls",
                "channel:read:predictions",
                "channel:read:goals",
                "channel:read:vips",
                "channel:read:charity",
                "channel:read:guest_star",
                "moderator:read:chatters",
                "moderator:read:shoutouts",
                "moderator:read:followers",
                "moderation:read",
                "channel:bot",
                "user:bot",
                "user:read:chat",
                "chat:edit",
                "user:write:chat",
                "user:read:emotes"
            ]).Wait();
        }
    }

    [Test, Order(1)]
    public void TestOAuthUserLogin()
    {
        Assert.That(_twitchApiService, Is.Not.Null);
        Assert.That(_twitchApiService.AccessToken, Is.Not.Empty);
    }
    
    [Test, Order(2)]
    public void TestGetChatters()
    {
        Assert.That(_twitchApiService, Is.Not.Null);
        Assert.That(_twitchApiService.AccessToken, Is.Not.Empty);
        
        Assert.DoesNotThrowAsync(async () =>
        {
            var chatters = await _twitchApiService.GetChattersAsync(BroadcasterId, ModeratorId);
            
            Assert.That(chatters, Is.Not.Null);
            Assert.That(chatters.Chatters, Is.Not.Count.Zero);
            
            foreach (var chatter in chatters.Chatters)
            {
                await TestContext.Out.WriteLineAsync($"Chatter: {chatter.UserId} {chatter.UserLogin} {chatter.UserName}");
                
                Assert.Multiple(() =>
                {
                    Assert.That(chatter, Is.Not.Null);
                    Assert.That(chatter.UserId, Is.Not.Empty);
                    Assert.That(chatter.UserLogin, Is.Not.Empty);
                    Assert.That(chatter.UserName, Is.Not.Empty);
                });
            }
        });
    }
    
    [Test, Order(3)]
    public void TestGetChannelFollowers()
    {
        Assert.That(_twitchApiService, Is.Not.Null);
        Assert.That(_twitchApiService.AccessToken, Is.Not.Empty);
        
        Assert.DoesNotThrowAsync(async () =>
        {
            var followers = await _twitchApiService.GetChannelFollowersAsync(BroadcasterId);
            
            Assert.That(followers, Is.Not.Null);
            Assert.That(followers.Followers, Is.Not.Count.Zero);
            
            foreach (var follower in followers.Followers)
            {
                await TestContext.Out.WriteLineAsync($"Follower: {follower.UserId} {follower.UserLogin} {follower.UserName} {follower.FollowedAt}");
                
                Assert.Multiple(() =>
                {
                    Assert.That(follower, Is.Not.Null);
                    Assert.That(follower.UserId, Is.Not.Empty);
                    Assert.That(follower.UserLogin, Is.Not.Empty);
                    Assert.That(follower.UserName, Is.Not.Empty);
                    Assert.That(follower.FollowedAt, Is.Not.EqualTo(default(DateTime)));
                });
            }
        });
    }
}