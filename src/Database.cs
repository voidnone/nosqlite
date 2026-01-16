using System.Collections.Concurrent;

namespace VoidNone.NoSQLite;

public class Database
{
    private readonly Connection connection;

    private readonly ConcurrentDictionary<string, dynamic> collections = [];
    private readonly string? path;

    internal Database(string? path)
    {
        this.path = path;
        connection = new(path);
    }

    public string? Path => path;

    public Collection<T>? GetCollection<T>()
    {
        collections.TryGetValue(typeof(T).Name, out var collection);
        return collection;
    }

    public Collection<T> GetCollectionRequired<T>()
    {
        return GetCollection<T>() ?? throw new CollectionNotFoundException(typeof(T).Name);
    }

    public Collection? GetCollection(string name)
    {
        collections.TryGetValue(name, out var result);
        return result;
    }

    public Collection GetCollectionRequired(string name)
    {
        return GetCollection(name) ?? throw new CollectionNotFoundException(name);
    }

    public Collection<T> GetOrCreateCollection<T>()
    {
        return collections.GetOrAdd(typeof(T).Name, (name) => new Collection<T>(connection));
    }

    public Collection GetOrCreateCollection(string name)
    {
        return collections.GetOrAdd(name, (name) => new Collection(connection, name));
    }

    public bool RemoveCollection<T>() => RemoveCollection(typeof(T).Name);

    public bool RemoveCollection(string name)
    {
        if (!collections.TryRemove(name, out var _)) return false;
        connection.Execute($"DROP TABLE IF EXISTS `{name}`");
        return true;
    }

    public static Database Create(string? path = null)
    {
        return new Database(path);
    }

    public static bool Remove(string path)
    {
        if (!File.Exists(path)) return false;
        File.Delete(path);
        return true;
    }
}