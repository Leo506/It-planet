using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ItPlanet.Web.Converters;

public class DateTimeConverter : JsonConverter<DateTime>
{
    private const string Pattern = "yyyy-MM-ddTHH:mmZ";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.ParseExact(reader.GetString()!, Pattern, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Pattern, CultureInfo.InvariantCulture));
    }
}