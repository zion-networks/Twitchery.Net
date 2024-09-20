# Twitchery.Net - A .NET library for Twitch
Freshly born library to access Twitch stuff, designed to be simple, efficient and easy to use.
We want you to be able to create your own Twitch bots, chat clients, stream viewers, and more with ease.

**This library is still under heavy development and not ready for production use.**

You can check the current implementation status here: [Implementation Status](RouteImplementations.md)

# Using Twitchery.Net: For Developers, Contributors and Testers

### Prerequisites

1. Clone the repository: `git clone git@github.com:zion-networks/Twitchery.Net.git`
2. Build the project: `dotnet build`

### Add dependencies to your project

1. Twitchery.Net: `dotnet add reference /path/to/Twitchery.Net/Twitchery.Net/bin/Debug/net8.0/Twitchery.Net.dll`
2. Newtonsoft.Json: `dotnet add package Newtonsoft.Json`
3. Microsoft.Extensions.Logging: `dotnet add package Microsoft.Extensions.Logging`
4. Microsoft.Extensions.Logging.Console: `dotnet add package Microsoft.Extensions.Logging.Console`

### Example Usage

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