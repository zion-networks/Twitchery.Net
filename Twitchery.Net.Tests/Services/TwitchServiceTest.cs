using TwitcheryNet.Extensions.TwitchApi;
using TwitcheryNet.Models.Helix;
using TwitcheryNet.Services.Implementation;

namespace TwitcheryNet.Tests.Services;

public class Tests
{
    private TwitchApiService _twitchApiService;

    private string _twitchClientId = string.Empty;
    private string _twitchAccessToken = string.Empty;
    private string _twitchBroadcasterId = string.Empty;
    private string _twitchModeratorId = string.Empty;
    
    [SetUp]
    public void Setup()
    {
        _twitchClientId = Environment.GetEnvironmentVariable("TWITCH_CLIENT_ID") ?? string.Empty;
        _twitchAccessToken = Environment.GetEnvironmentVariable("TWITCH_ACCESS_TOKEN") ?? string.Empty;
        _twitchBroadcasterId = Environment.GetEnvironmentVariable("TWITCH_BROADCASTER_ID") ?? string.Empty;
        _twitchModeratorId = Environment.GetEnvironmentVariable("TWITCH_MODERATOR_ID") ?? string.Empty;
        
        Assert.That(_twitchClientId, Is.Not.Empty);
        Assert.That(_twitchAccessToken, Is.Not.Empty);
        Assert.That(_twitchBroadcasterId, Is.Not.Empty);
        Assert.That(_twitchModeratorId, Is.Not.Empty);
        
        _twitchApiService = new TwitchApiService
        {
            ClientId = _twitchClientId,
            ClientAccessToken = _twitchAccessToken,
            ClientScopes =
            {
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
            }
        };
        
        Assert.That(_twitchApiService, Is.Not.Null);
    }

    [Test]
    public void TestGetChatters()
    {
        Assert.DoesNotThrowAsync(async () =>
        {
            var chatters = await _twitchApiService.GetChattersAsync(_twitchBroadcasterId, _twitchModeratorId);
            
            Assert.That(chatters, Is.Not.Null);
            
            foreach (var chatter in chatters.Chatters)
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
            var followers = await _twitchApiService.GetAllChannelFollowersAsync(_twitchBroadcasterId);
            
            Assert.That(followers, Is.Not.Null);
            
            foreach (var follower in followers.Followers)
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
        Assert.DoesNotThrowAsync(async () =>
        {
            var streams = await _twitchApiService.GetStreamsAsync(userLogins: [ "GronkhTV" ]);
            
            Assert.That(streams, Is.Not.Null);
            
            foreach (var stream in streams.Streams)
            {
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
            }
        });
    }
    
    [Test]
    public void TestGetChannelInformation()
    {
        Assert.DoesNotThrowAsync(async () =>
        {
            var channelInfos = await _twitchApiService.GetChannelInformationAsync(_twitchBroadcasterId);
            
            Assert.That(channelInfos, Is.Not.Null);
            
            foreach (var channelInfo in channelInfos.ChannelInformations)
            {
                Assert.That(channelInfo, Is.Not.Null);
                Assert.That(channelInfo.BroadcasterId, Is.Not.Empty);
                Assert.That(channelInfo.BroadcasterLogin, Is.Not.Empty);
                Assert.That(channelInfo.BroadcasterName, Is.Not.Empty);
                Assert.That(channelInfo.BroadcasterLanguage, Is.Not.Empty);
                Assert.That(channelInfo.GameName, Is.Not.Empty);
                Assert.That(channelInfo.GameId, Is.Not.Empty);
                Assert.That(channelInfo.Title, Is.Not.Empty);
            }
        });
    }
    
    [Test]
    public void TestSendChatMessageUser()
    {
        Assert.DoesNotThrowAsync(async () =>
        {
            var sentMessageResponse = await _twitchApiService.SendChatMessageUserAsync(_twitchBroadcasterId, _twitchBroadcasterId, "Hello, World from Twitchery.Net!");
            
            Assert.That(sentMessageResponse, Is.Not.Null);
            
            foreach (var sentMessage in sentMessageResponse.SentMessages)
            {
                Assert.That(sentMessage.IsSent, Is.True);
                Assert.That(sentMessage.MessageId, Is.Not.Empty);
                Assert.That(sentMessage.DropReason, Is.Null);
            }
        });
    }

    [Test]
    public void TestGetUsers()
    {
        Assert.DoesNotThrowAsync(async () =>
        {
            var users = await _twitchApiService.GetUsersAsync(userLogins: [ "GronkhTV" ]);
            
            Assert.That(users, Is.Not.Null); 
            Assert.That(users.Users, Is.Not.Null.And.Count.EqualTo(1)); 
            
            foreach (var user in users.Users)
            {
                Assert.That(user, Is.Not.Null);
                Assert.That(user.Id, Is.Not.Empty);
                Assert.That(user.Login, Is.Not.Empty);
                Assert.That(user.DisplayName, Is.Not.Empty);
                Assert.That(user.Type, Is.Not.Null);
                Assert.That(user.BroadcasterType, Is.EqualTo(BroadcasterType.Partner));
                Assert.That(user.Description, Is.Not.Null);
                Assert.That(user.ProfileImageUrl, Is.Not.Null);
                Assert.That(user.OfflineImageUrl, Is.Not.Null);
                
                if (_twitchApiService.ClientScopes.Contains("user:read:email"))
                {
                    Assert.That(user.Email, Is.Not.Empty);
                }
                else
                {
                    Assert.That(user.Email, Is.Null.Or.Empty);
                }
            }
        });
    }
}