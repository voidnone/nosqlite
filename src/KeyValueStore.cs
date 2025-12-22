namespace VoidNone.Nosqlite;

public class KeyValueStore : NosqliteStore
{
    public KeyValueStore(string path) : base(path)
    {
        CreateTable();
    }

    public string? Get(string key)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Value FROM KeyValue WHERE Key = @Key LIMIT 1";
        command.Parameters.AddWithValue("@Key", key);
        using var reader = command.ExecuteReader();
        var hasValue = reader.Read();
        if (!hasValue) return null;
        return reader.GetString(0);
    }

    public void Set(string key, string value)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT OR REPLACE INTO KeyValue (Key, Value, CreationTime) VALUES (@Key, @Value, @CreationTime)";
        command.Parameters.AddWithValue("@Key", key);
        command.Parameters.AddWithValue("@Value", value);
        command.Parameters.AddWithValue("@CreationTime", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        command.ExecuteNonQuery();
    }

    private void CreateTable()
    {
        Execute($"""
        CREATE TABLE IF NOT EXISTS `KeyValue` (
            Key TEXT PRIMARY KEY,
            Value TEXT NOT NULL,
            CreationTime INTEGER NOT NULL
        ) WITHOUT ROWID;

        CREATE INDEX IF NOT EXISTS KeyValue_OwnerId_INDEX
        ON `KeyValue`(CreationTime);
        """);
    }
}