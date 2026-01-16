using System.Collections.Concurrent;
using Microsoft.Data.Sqlite;

namespace VoidNone.NoSQLite;

public class Database
{
    private readonly ConcurrentDictionary<string, dynamic> collections = [];

    public Connection Connection { get; }

    internal Database(string? path)
    {
        Connection = new(path);
    }

    public Collection<T>? GetCollection<T>()
    {
        collections.TryGetValue(typeof(T).Name, out var collection);
        return collection;
    }

    public Collection<T> GetRequiredCollection<T>()
    {
        return GetCollection<T>() ?? throw new CollectionNotFoundException(typeof(T).Name);
    }

    public Collection? GetCollection(string name)
    {
        collections.TryGetValue(name, out var result);
        return result;
    }

    public Collection GetRequiredCollection(string name)
    {
        return GetCollection(name) ?? throw new CollectionNotFoundException(name);
    }

    public Collection<T> GetOrCreateCollection<T>()
    {
        return collections.GetOrAdd(typeof(T).Name, (name) => new Collection<T>(Connection));
    }

    public Collection GetOrCreateCollection(string name)
    {
        return collections.GetOrAdd(name, (name) => new Collection(Connection, name));
    }

    public bool RemoveCollection<T>() => RemoveCollection(typeof(T).Name);

    public bool RemoveCollection(string name)
    {
        if (!collections.TryRemove(name, out var _)) return false;
        Connection.Execute($"DROP TABLE IF EXISTS `{name}`");
        return true;
    }

    public static Database Create(string? path = null)
    {
        return new Database(path);
    }

    public static bool Remove(string path)
    {
        if (!File.Exists(path)) return false;
        var connection = new Connection(path).OpenConnection();
        SqliteConnection.ClearPool(connection);
        File.Delete(path);
        return true;
    }
}