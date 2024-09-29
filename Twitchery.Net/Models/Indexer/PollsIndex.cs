using Microsoft.Extensions.DependencyInjection;
using TwitcheryNet.Attributes;
using TwitcheryNet.Models.Helix.Polls;
using TwitcheryNet.Services.Implementations;

namespace TwitcheryNet.Models.Indexer;

public class PollsIndex
{
    private Twitchery Twitch { get; }
    
    [ActivatorUtilitiesConstructor]
    public PollsIndex(Twitchery api)
    {
        Twitch = api;
    }
    
    [ApiRoute("POST", "polls", "channel:manage:polls")]
    [RequiresToken(TokenType.UserAccess)]
    public async Task<CreatePollResponseBody?> CreatePollAsync(CreatePollRequestBody requestBody, CancellationToken cancellationToken = default)
    {
        return await Twitch.PostTwitchApiAsync<CreatePollRequestBody, CreatePollResponseBody>(requestBody, typeof(PollsIndex), cancellationToken);
    }
    
    public async Task<CreatePollResponseBody?> CreatePollAsync(string title, IEnumerable<string> sChoices, int duration = 300, string? broadcasterId = null, CancellationToken cancellationToken = default)
    {
        var bid = broadcasterId ?? Twitch.Me?.Id ?? throw new InvalidOperationException("No broadcaster ID provided.");
        var choices = sChoices.Select(choice => new PollChoice(choice)).ToArray();
        var requestBody = new CreatePollRequestBody(bid, title, choices)
        {
            Duration = duration
        };
        
        return await Twitch.PostTwitchApiAsync<CreatePollRequestBody, CreatePollResponseBody>(requestBody, typeof(PollsIndex), cancellationToken);
    }
}