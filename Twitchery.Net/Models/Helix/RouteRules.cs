namespace TwitcheryNet.Models.Helix;

[Flags]
public enum RouteRules
{
    None              = 0,
    RequiresOwner     = 1 << 0,
    RequiresModerator = 1 << 1
}