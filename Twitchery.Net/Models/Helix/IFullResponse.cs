namespace TwitcheryNet.Models.Helix;

public interface IFullResponse<in T> where T : class
{
    public void Add(T item);
}