using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VoidNone.NoSQLite;

public static class JsonHelper
{
    private static readonly JsonSerializerOptions defaultOptions = new(JsonSerializerDefaults.Web);
    private static readonly JsonSerializerOptions readableOptions = new(JsonSerializerDefaults.Web);
    public static JsonSerializerOptions DefaultOptions => defaultOptions;
    public static JsonSerializerOptions ReadableOptions => readableOptions;

    static JsonHelper()
    {
        ConfigureDefaultOptions(defaultOptions);
        ConfigureReadableOptions(readableOptions);
    }

    public static void ConfigureDefaultOptions(JsonSerializerOptions options)
    {
        options.PropertyNameCaseInsensitive = true;
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.NumberHandling = JsonNumberHandling.AllowReadingFromString;
        options.Converters.Add(new JsonStringEnumConverter());
    }

    public static void ConfigureReadableOptions(JsonSerializerOptions options)
    {
        ConfigureDefaultOptions(options);
        options.AllowTrailingCommas = true;
        options.ReadCommentHandling = JsonCommentHandling.Skip;
        options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        options.WriteIndented = true;
    }

    #region Deserialize

    public static T? Deserialize<T>(Stream stream) => JsonSerializer.Deserialize<T>(stream, defaultOptions);

    public static T? DeserializeReadable<T>(Stream stream) => JsonSerializer.Deserialize<T>(stream, readableOptions);

    public static T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, defaultOptions);

    public static T? DeserializeReadable<T>(string json) => JsonSerializer.Deserialize<T>(json, readableOptions);

    public static ValueTask<T?> DeserializeAsync<T>(Stream stream, CancellationToken token = default) => JsonSerializer.DeserializeAsync<T>(stream, defaultOptions, token);

    public static ValueTask<T?> DeserializeReadableAsync<T>(Stream stream, CancellationToken token = default) => JsonSerializer.DeserializeAsync<T>(stream, readableOptions, token);

    public static ValueTask<T?> DeserializeAsync<T>(string path, CancellationToken token = default) => DeserializeAsync<T>(path, defaultOptions, token);

    public static ValueTask<T?> DeserializeReadableAsync<T>(string path, CancellationToken token = default) => DeserializeAsync<T>(path, readableOptions, token);

    private static async ValueTask<T?> DeserializeAsync<T>(string path, JsonSerializerOptions options, CancellationToken token)
    {
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return await JsonSerializer.DeserializeAsync<T>(stream, options, token);
    }

    #endregion

    #region Serialize

    public static string Serialize(object value) => JsonSerializer.Serialize(value, defaultOptions);
    public static string SerializeReadable(object value) => JsonSerializer.Serialize(value, readableOptions);

    public static void Serialize(Stream stream, object value) => JsonSerializer.Serialize(stream, value, defaultOptions);
    public static void SerializeReadable(Stream stream, object value) => JsonSerializer.Serialize(stream, value, readableOptions);

    public static Task SerializeAsync(Stream stream, object value, CancellationToken token = default) => JsonSerializer.SerializeAsync(stream, value, defaultOptions, token);

    public static Task SerializeReadableAsync(Stream stream, object value, CancellationToken token = default) => JsonSerializer.SerializeAsync(stream, value, readableOptions, token);

    public static Task SerializeAsync(string path, object value, CancellationToken token = default) => SerializeAsync(path, value, defaultOptions, token);

    public static Task SerializeReadableAsync(string path, object value, CancellationToken token = default) => SerializeAsync(path, value, readableOptions, token);

    private static async Task SerializeAsync(string path, object value, JsonSerializerOptions options, CancellationToken token)
    {
        using var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        stream.SetLength(0);
        await JsonSerializer.SerializeAsync(stream, value, options, token);
    }

    #endregion
}