namespace VoidNone.NoSQLite;

public class KeyValueStore : StoreBase
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
        var parameters = new Dictionary<string, object>
        {
            {"@Key", key },
            {"@Value", value },
            {"@CreationTime", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() },
        };
        
        Execute("""
        INSERT OR REPLACE INTO KeyValue 
            (Key, Value, CreationTime) 
        VALUES 
            (@Key, @Value, @CreationTime)
        """, parameters);
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