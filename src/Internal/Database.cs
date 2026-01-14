using System.Collections.Concurrent;

namespace VoidNone.NoSQLite.Internal;

internal class Database(string path) : IDatabase
{
    private readonly Connection connection = new(path);

    private readonly ConcurrentDictionary<string, dynamic> collections = [];

    public string Path => path;

    public ICollection<T>? GetCollection<T>()
    {
        collections.TryGetValue(typeof(T).Name, out var collection);
        return collection;
    }

    public ICollection<T> GetCollectionRequired<T>()
    {
        return GetCollection<T>() ?? throw new CollectionNotFoundException(typeof(T).Name);
    }

    public ICollection? GetCollection(string name)
    {
        collections.TryGetValue(name, out var result);
        return result;
    }

    public ICollection GetCollectionRequired(string name)
    {
        return GetCollection(name) ?? throw new CollectionNotFoundException(name);
    }

    public ICollection<T> GetOrCreateCollection<T>()
    {
        return collections.GetOrAdd(typeof(T).Name, (name) => new Collection<T>(connection));
    }

    public ICollection GetOrCreateCollection(string name)
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
}