namespace TwitcheryNet.Services.Interfaces;

public interface IBaseTwitchService
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? AccessToken { get; set; }
}