namespace TwitcheryNet.Services.Interfaces;

public interface IHasTwitchery
{
    public ITwitchery? Twitch { get; set; }
}