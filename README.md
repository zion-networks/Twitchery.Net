# Twitchery.Net - A .NET library for Twitch
Freshly born library to access Twitch stuff, designed to be simple, efficient and easy to use.
We want you to be able to create your own Twitch bots, chat clients, stream viewers, and more with ease.

> [!WARNING]  
>  **This library is still under heavy development and not ready for production use.**

You can check the current implementation status here: [Implementation Status](RouteImplementations.md)

# Using Twitchery.Net: For Developers, Contributors and Testers

## Getting Started as a Developer

1. Create a new Twitch application at [Twitch Developer Console](https://dev.twitch.tv/console/apps)
2. Create a new .NET8+ project or open an existing one
3. Install the library from NuGet: `dotnet add package TwitcheryNet --prerelease`
4. Start coding!

### Example Usage

```csharp
using TwitcheryNet.Models.Helix.Channels;
using TwitcheryNet.Services.Implementations;

const string myClientId = "your-client-id"; // Get this from your Twitch application
const string myRedirectUri = "http://localhost:8181"; // Must match the one in your Twitch application
var myScopes = new[]
{
    "chat:read",
    "chat:edit",
    "user:read:chat",
    "channel:moderate",
    "channel:read:subscriptions",
    "moderator:read:followers"
};

// Create a new Twitchery instance
var twitchery = new Twitchery();

// Authenticate with the user's default browser (Windows, Linux and OSX supported)
// If none is available, a URL will be printed to the console
await twitchery.UserBrowserAuthAsync(myClientId, myRedirectUri, myScopes);

// Print the user's display name
Console.WriteLine("Logged in as: " + twitchery.Me!.DisplayName);

// Get the authenticated user's channel
var myChannel = twitchery.Me.Channel;

// How many followers do I have?
// This will fetch all followers, so it may take a while if you have a lot
// In a future version, you'll be able to access the total amount and the recent followers more easy
var followerCount = 0;
Follower lastFollower = null;
await foreach (var follower in myChannel.Followers)
{
    // The first follower in the list is the most recent one
    if (lastFollower is null)
    {
        lastFollower = follower;
    }
    
    followerCount++;
}

Console.WriteLine("I have {0} followers", followerCount);

// Am I streaming right now?
var myStream = twitchery.Streams[twitchery.Me!.Login];
if (myStream is null)
{
    Console.WriteLine("I'm not streaming right now");
}
else
{
    Console.WriteLine("I'm streaming {0} for {1} viewers!", myStream.GameName, myStream.ViewerCount);
}

// Who was the last person to follow me?
Console.WriteLine("My last follower was {0}, who followed on {1}!", lastFollower.UserName, lastFollower.FollowedAt);

// Listen for events
myChannel.ChatMessage += (sender, e) =>
{
    Console.WriteLine("{0} said: {1}", e.ChatterUserName, e.Message.Text);
    return Task.CompletedTask;
};

myChannel.Follow += (sender, e) =>
{
    Console.WriteLine("{0} followed me!", e.UserName);
    return Task.CompletedTask;
};

myChannel.Subscribe += (sender, e) =>
{
    Console.WriteLine("{0} subscribed to me!", e.UserName);
    return Task.CompletedTask;
};

Console.WriteLine("Press any key to exit...");

// This is required to keep the bot running until the user presses a key
// To keep the bot running indefinitely, you can use `await Task.Delay(-1);`
await Task.Run(Console.ReadKey);

// Yes, it's that simple!
```

## Getting Started as a Contributor

Follow the contribution guidelines in [CONTRIBUTING.md](CONTRIBUTING.md) to get started.