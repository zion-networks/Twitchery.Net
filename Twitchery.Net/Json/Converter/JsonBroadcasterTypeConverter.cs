using Newtonsoft.Json;
using TwitcheryNet.Models.Helix;

namespace TwitcheryNet.Json.Converter;

public class JsonBroadcasterTypeConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        switch (value)
        {
            case null:
                writer.WriteValue("");
                break;
            
            case BroadcasterType broadcasterType:
                writer.WriteValue(broadcasterType switch
                {
                    BroadcasterType.Partner => "partner",
                    BroadcasterType.Affiliate => "affiliate",
                    _ => ""
                });
                break;
        }
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.Value is null)
        {
            return BroadcasterType.Normal;
        }

        return reader.Value switch
        {
            "partner" => BroadcasterType.Partner,
            "affiliate" => BroadcasterType.Affiliate,
            _ => BroadcasterType.Normal
        };
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(BroadcasterType);
    }
}