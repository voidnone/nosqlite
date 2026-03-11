using System.Text.Json;
using System.Text.Json.Serialization;

namespace VoidNone.NoSQLite.Internal;

internal static class JsonSerializerOptionsExtensions
{
    private static readonly JsonSerializerOptions database;
    static JsonSerializerOptionsExtensions()
    {
        database = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        database.Converters.Add(new DateTimeConverter());
        database.Converters.Add(new NullableDateTimeConverter());
        database.Converters.Add(new DateTimeOffsetConverter());
        database.Converters.Add(new NullableDateTimeOffsetConverter());
        database.Converters.Add(new TimeSpanConverter());
        database.Converters.Add(new NullableTimeSpanConverter());
    }

    extension(JsonSerializerOptions)
    {
        internal static JsonSerializerOptions Database => database;
    }
}

internal class DateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var ms = reader.GetInt64();
        return DateTimeOffset.FromUnixTimeMilliseconds(ms).UtcDateTime;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var ms = new DateTimeOffset(value.ToUniversalTime()).ToUnixTimeMilliseconds();
        writer.WriteNumberValue(ms);
    }
}

internal class NullableDateTimeConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        var ms = reader.GetInt64();
        return DateTimeOffset.FromUnixTimeMilliseconds(ms).UtcDateTime;
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value == null)
            writer.WriteNullValue();
        else
            writer.WriteNumberValue(new DateTimeOffset(value.Value.ToUniversalTime()).ToUnixTimeMilliseconds());
    }
}

internal class DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var ms = reader.GetInt64();
        return DateTimeOffset.FromUnixTimeMilliseconds(ms);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.ToUnixTimeMilliseconds());
    }
}

internal class NullableDateTimeOffsetConverter : JsonConverter<DateTimeOffset?>
{
    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        return DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64());
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (value == null)
            writer.WriteNullValue();
        else
            writer.WriteNumberValue(value.Value.ToUnixTimeMilliseconds());
    }
}

internal class TimeSpanConverter : JsonConverter<TimeSpan>
{
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return TimeSpan.FromMilliseconds(reader.GetInt64());
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue((long)value.TotalMilliseconds);
    }
}

internal class NullableTimeSpanConverter : JsonConverter<TimeSpan?>
{
    public override TimeSpan? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        return TimeSpan.FromMilliseconds(reader.GetInt64());
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
    {
        if (value == null)
            writer.WriteNullValue();
        else
            writer.WriteNumberValue((long)value.Value.TotalMilliseconds);
    }
}