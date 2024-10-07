using Newtonsoft.Json;

namespace TwitcheryNet.Net.EventSub.EventArgs.Shared;

[JsonObject]
public class Message
{
    [JsonProperty("text")]
    public string Text { get; set; } = string.Empty;
    
    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonProperty("fragments")]
    public List<Fragment> Fragments { get; set; } = [];

    public override string ToString()
    {
        return Text;
    }
    
    public static implicit operator string(Message message) => message.Text;
}