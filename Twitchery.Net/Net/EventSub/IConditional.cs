using TwitcheryNet.Models.Helix.EventSub.Subscriptions;

namespace TwitcheryNet.Net.EventSub;

public interface IConditional
{
    public EventSubCondition ToCondition();
}