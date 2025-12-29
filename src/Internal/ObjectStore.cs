using System.Collections.Concurrent;

namespace VoidNone.NoSQLite.Internal;

internal class ObjectStore(string path) : StoreBase(path), IObjectStore
{
    private readonly ConcurrentDictionary<string, dynamic> collections = [];

    public ICollection<T>? Get<T>()
    {
        collections.TryGetValue(typeof(T).Name, out var collection);
        return collection;
    }

    public ICollection<T> GetRequired<T>()
    {
        return Get<T>() ?? throw new CollectionNotFoundException(typeof(T).Name);
    }

    public ICollection? Get(string collection)
    {
        collections.TryGetValue(collection, out var result);
        return result;
    }

    public ICollection GetRequired(string collection)
    {
        return Get(collection) ?? throw new CollectionNotFoundException(collection);
    }

    public ICollection<T> GetOrCreate<T>()
    {
        return collections.GetOrAdd(typeof(T).Name, (name) => new Collection<T>(this));
    }

    public ICollection GetOrCreate(string collection)
    {
        return collections.GetOrAdd(collection, (name) => new Collection(this, name));
    }

    public void Remove<T>() => Remove(typeof(T).Name);

    public void Remove(string collection)
    {
        if (!collections.TryRemove(collection, out var _)) return;
        Execute($"DROP TABLE IF EXISTS `{collection}`");
    }

    public void Remove() => File.Delete(path);
}