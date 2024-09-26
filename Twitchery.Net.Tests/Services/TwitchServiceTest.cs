using System.Diagnostics;
using TwitcheryNet.Models.Helix;
using TwitcheryNet.Services.Implementations;

namespace TwitcheryNet.Tests.Services;

public class Tests
{
    private Twitchery _twitchery;

    private string? _twitchClientId;
    private string? _twitchAccessToken;
    private string? _twitchRedirectUri;
    private string? _twitchBroadcasterId;
    private string? _twitchModeratorId;

    private readonly string[] _scopes =
    [
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
    ];

    private static string? GetEnvironmentVariable(string name)
    {
        var process = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        var user = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User);
        var machine = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Machine);
        
        if (string.IsNullOrWhiteSpace(process) is false)
            return process;
        
        if (string.IsNullOrWhiteSpace(user) is false)
            return user;

        if (string.IsNullOrWhiteSpace(machine) is false)
            return machine;
        
        return null;
    }
    
    [OneTimeSetUp]
    public void Setup()
    {
        Trace.Listeners.Add(new ConsoleTraceListener());
        
        _twitchClientId = GetEnvironmentVariable("TWITCH_CLIENT_ID");
        _twitchAccessToken = GetEnvironmentVariable("TWITCH_ACCESS_TOKEN");
        _twitchRedirectUri = GetEnvironmentVariable("TWITCH_REDIRECT_URI");
        _twitchBroadcasterId = GetEnvironmentVariable("TWITCH_BROADCASTER_ID");
        _twitchModeratorId = GetEnvironmentVariable("TWITCH_MODERATOR_ID");
        
        var isGithubActions = GetEnvironmentVariable("GITHUB_ACTIONS") == "true";
        
        Assert.That(_twitchClientId, Is.Not.Null.And.Not.Empty);
        Assert.That(_twitchAccessToken, Is.Not.Null.And.Not.Empty);
        Assert.That(_twitchRedirectUri, Is.Not.Null.And.Not.Empty);
        Assert.That(_twitchBroadcasterId, Is.Not.Null.And.Not.Empty);
        Assert.That(_twitchModeratorId, Is.Not.Null.And.Not.Empty);
        
        _twitchery = new Twitchery
        {
            ClientId = _twitchClientId,
            ClientAccessToken = _twitchAccessToken
        };
        
        _twitchery.ClientScopes.AddRange(_scopes);

        if (isGithubActions is false)
        {
            _twitchery.ClientAccessToken = null;
            _twitchery.UserBrowserAuthAsync(_twitchRedirectUri, _scopes).Wait();
        }
        
        Assert.That(_twitchery, Is.Not.Null);
    }
    
    [OneTimeTearDown]
    public void EndTest()
    {
        Trace.Flush();
    }

    [Test]
    public void TestGetChatters()
    {
        Assert.DoesNotThrow(() =>
        {
            var chatters = _twitchery.Chat[_twitchBroadcasterId, _twitchModeratorId];
            
            Assert.That(chatters, Is.Not.Null);
            
            foreach (var chatter in chatters)
            {
                Assert.That(chatter, Is.Not.Null);
                Assert.That(chatter.UserId, Is.Not.Empty);
                Assert.That(chatter.UserLogin, Is.Not.Empty);
                Assert.That(chatter.UserName, Is.Not.Empty);
            }
        });
    }
    
    [Test]
    public void TestGetChannelFollowers()
    {
        Assert.DoesNotThrowAsync(async () =>
        {
            var followers = _twitchery.Channels[_twitchBroadcasterId]?.Followers;
            
            Assert.That(followers, Is.Not.Null);
            
            await foreach (var follower in followers)
            {
                Assert.That(follower, Is.Not.Null);
                Assert.That(follower.UserId, Is.Not.Empty);
                Assert.That(follower.UserLogin, Is.Not.Empty);
                Assert.That(follower.UserName, Is.Not.Empty);
                Assert.That(follower.FollowedAt, Is.Not.EqualTo(default(DateTime)));
            }
        });
    }
    
    [Test]
    public void TestGetStreams()
    {
        Assert.DoesNotThrow(() =>
        {
            var stream = _twitchery.Streams["GronkhTV"];
            
            Assert.That(stream, Is.Not.Null);
            Assert.That(stream.Id, Is.Not.Empty);
            Assert.That(stream.UserId, Is.Not.Empty);
            Assert.That(stream.UserLogin, Is.Not.Empty);
            Assert.That(stream.UserName, Is.Not.Empty);
            Assert.That(stream.ViewerCount, Is.Positive.Or.Zero);
            Assert.That(stream.StartedAt, Is.Not.EqualTo(default(DateTime)));
            Assert.That(stream.Language, Is.Not.Empty);
            Assert.That(stream.ThumbnailUrl, Is.Not.Empty);
            Assert.That(stream.Tags, Is.Not.Count.Negative);
            Assert.That(stream.TagIds, Is.Not.Count.Negative);
            Assert.That(stream.GameId, Is.Not.Empty);
            Assert.That(stream.GameName, Is.Not.Empty);
            Assert.That(stream.Type, Is.Not.Empty);
            Assert.That(stream.Title, Is.Not.Empty);
        });
    }
    
    [Test]
    public void TestGetChannelInformation()
    {
        Assert.DoesNotThrow(() =>
        {
            var channelInfo = _twitchery.Channels[_twitchBroadcasterId];
            
            Assert.That(channelInfo, Is.Not.Null);
            Assert.That(channelInfo.BroadcasterId, Is.Not.Empty);
            Assert.That(channelInfo.BroadcasterLogin, Is.Not.Empty);
            Assert.That(channelInfo.BroadcasterName, Is.Not.Empty);
            Assert.That(channelInfo.BroadcasterLanguage, Is.Not.Empty);
            Assert.That(channelInfo.GameName, Is.Not.Empty);
            Assert.That(channelInfo.GameId, Is.Not.Empty);
            Assert.That(channelInfo.Title, Is.Not.Empty);
        });
    }
    
    //[Test]
    //public void TestSendChatMessageUser()
    //{
    //    Assert.DoesNotThrowAsync(async () =>
    //    {
    //        var msgRequest = new SendChatMessageRequestBody(
    //            _twitchBroadcasterId,
    //            _twitchBroadcasterId,
    //            "Hello, World! Twitchery.Net successfully passed another test!");
    //        var sentMessageResponse = await _twitchery.Chat.SendChatMessageUserAsync(msgRequest);
    //        
    //        Assert.That(sentMessageResponse, Is.Not.Null);
    //        
    //        foreach (var sentMessage in sentMessageResponse.SentMessages)
    //        {
    //            Assert.That(sentMessage.IsSent, Is.True);
    //            Assert.That(sentMessage.MessageId, Is.Not.Empty);
    //            Assert.That(sentMessage.DropReason, Is.Null);
    //        }
    //    });
    //}

    [Test]
    public void TestGetUsers()
    {
        Assert.DoesNotThrow(() =>
        {
            var user = _twitchery.Users["GronkhTV"];
            
            Assert.That(user, Is.Not.Null);
            Assert.That(user.Id, Is.Not.Empty);
            Assert.That(user.Login, Is.Not.Empty);
            Assert.That(user.DisplayName, Is.Not.Empty);
            Assert.That(user.Type, Is.Not.Null);
            Assert.That(user.BroadcasterType, Is.EqualTo(BroadcasterType.Partner));
            Assert.That(user.Description, Is.Not.Null);
            Assert.That(user.ProfileImageUrl, Is.Not.Null);
            Assert.That(user.OfflineImageUrl, Is.Not.Null);
            
            if (_twitchery.ClientScopes.Contains("user:read:email"))
            {
                Assert.That(user.Email, Is.Not.Empty);
            }
            else
            {
                Assert.That(user.Email, Is.Null.Or.Empty);
            }
        });
    }
}