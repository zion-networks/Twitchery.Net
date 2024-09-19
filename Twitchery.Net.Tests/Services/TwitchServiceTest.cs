using TwitcheryNet.Extensions.TwitchApi;
using TwitcheryNet.Services.Implementation;

namespace TwitcheryNet.Tests.Services;

public class Tests
{
    private TwitchApiService? _twitchApiService;

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
        
        Assert.Multiple(() =>
        {
            Assert.That(_twitchClientId, Is.Not.Empty);
            Assert.That(_twitchAccessToken, Is.Not.Empty);
            Assert.That(_twitchBroadcasterId, Is.Not.Empty);
            Assert.That(_twitchModeratorId, Is.Not.Empty);
        });
        
        _twitchApiService ??= new TwitchApiService
        {
            ClientId = _twitchClientId,
            AccessToken = _twitchAccessToken
        };
        
        Assert.That(_twitchApiService, Is.Not.Null);
    }

    [Test, Order(2)]
    public void TestGetChatters()
    {
        Assert.That(_twitchApiService, Is.Not.Null);
        Assert.That(_twitchApiService.AccessToken, Is.Not.Empty);
        
        Assert.DoesNotThrowAsync(async () =>
        {
            var chatters = await _twitchApiService.GetChattersAsync(_twitchBroadcasterId, _twitchModeratorId);
            
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
            var followers = await _twitchApiService.GetChannelFollowersAsync(_twitchBroadcasterId);
            
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
    
    [Test, Order(4)]
    public void TestGetStreams()
    {
        Assert.That(_twitchApiService, Is.Not.Null);
        Assert.That(_twitchApiService.AccessToken, Is.Not.Empty);
        
        Assert.DoesNotThrowAsync(async () =>
        {
            var streams = await _twitchApiService.GetStreamsAsync(userLogins: [ "GronkhTV" ]);
            
            Assert.That(streams, Is.Not.Null);
            
            foreach (var stream in streams.Streams)
            {
                await TestContext.Out.WriteLineAsync($"Stream: {stream.UserName} started at {stream.StartedAt} and has {stream.ViewerCount} viewers right now");
                
                Assert.Multiple(() =>
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
                });
            }
        });
    }
    
    [Test, Order(5)]
    public void TestGetChannelInformation()
    {
        Assert.That(_twitchApiService, Is.Not.Null);
        Assert.That(_twitchApiService.AccessToken, Is.Not.Empty);
        
        Assert.DoesNotThrowAsync(async () =>
        {
            var channelInfos = await _twitchApiService.GetChannelInformationAsync(_twitchBroadcasterId);
            
            Assert.That(channelInfos, Is.Not.Null);
            
            foreach (var channelInfo in channelInfos.ChannelInformations)
            {
                await TestContext.Out.WriteLineAsync($"Channel Info: {channelInfo.BroadcasterName} currently streaming {channelInfo.GameName} with title {channelInfo.Title}");
                
                Assert.Multiple(() =>
                {
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
        });
    }
    
    [Test, Order(6)]
    public void TestSendChatMessageUser()
    {
        Assert.That(_twitchApiService, Is.Not.Null);
        Assert.That(_twitchApiService.AccessToken, Is.Not.Empty);
        
        Assert.DoesNotThrowAsync(async () =>
        {
            var sentMessageResponse = await _twitchApiService.SendChatMessageUserAsync(_twitchBroadcasterId, _twitchBroadcasterId, "Hello, World from Twitchery.Net!");
            
            Assert.That(sentMessageResponse, Is.Not.Null);
            
            foreach (var sentMessage in sentMessageResponse.SentMessages)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(sentMessage.IsSent, Is.True);
                    Assert.That(sentMessage.MessageId, Is.Not.Empty);
                    Assert.That(sentMessage.DropReason, Is.Null);
                });
            }
        });
    }
}