# Twitchery.Net - A .NET library for Twitch
Freshly born library to access Twitch stuff, designed to be simple, efficient and easy to use.
We want you to be able to create your own Twitch bots, chat clients, stream viewers, and more with ease.

> [!WARNING]  
>  **This library is still under heavy development and not ready for production use.**

You can check the current implementation status here: [Implementation Status](RouteImplementations.md)

# Using Twitchery.Net: For Developers, Contributors and Testers

## Getting Started

1. Create a new Twitch application at [Twitch Developer Console](https://dev.twitch.tv/console/apps)
2. Create a new .NET8+ project or open an existing one
3. Install the library from NuGet: `dotnet add package TwitcheryNet --prerelease`
4. Start coding!

## Example Usage

```csharp
using TwitcheryNet.Services.Implementations;

var myClientId = "your-client-id";
var myRedirectUri = "http://localhost:8080";
var myScopes = new string[] { "chat:read", "chat:edit", "channel:moderate" };

var twitchery = new Twitchery { ClientId = myClientId };

// Authenticate with the user's default browser (Windows, Linux and OSX supported)
await twitchery.UserBrowserAuthAsync(redirectUri, scopes);

// Print the user's display name
Console.WriteLine("Logged in as: " + twitchery.Me!.DisplayName);

// How many followers do I have?
Console.WriteLine("I have {0} followers", twitchery.ChannelFollowers[twitchery.Me!.Id].Count);

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
var lastFollower = twitchery.ChannelFollowers[twitchery.Me!.Id].First(); // First, because the list is sorted by follow date
Console.WriteLine("My last follower was {0}, who followed on {1}!", lastFollower.UserName, lastFollower.FollowedAt);

// Yes, it's that simple!
```