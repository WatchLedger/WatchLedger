using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using WatchCollection.Shared.Enums;
using WatchCollection.Shared.Extensions;

namespace WatchCollection.Shared.Converters;

public class AdvertisementStatusJsonConverter : JsonConverter<AdvertisementStatus>
{
    public override AdvertisementStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString() ?? throw new JsonException("AdvertisementStatus value cannot be null.");
        try
        {
            return stringValue.ParseAdvertisementStatus();
        }
        catch (InvalidOperationException ex)
        {
            throw new JsonException(ex.Message);
        }
    }

    public override void Write(Utf8JsonWriter writer, AdvertisementStatus value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
