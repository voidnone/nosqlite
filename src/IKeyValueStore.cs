namespace VoidNone.NoSQLite;

public interface IKeyValueStore
{
    string? Get(string key);
    void Set(string key, string value);
}