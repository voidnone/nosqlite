using System.Collections.Concurrent;

namespace VoidNone.Nosqlite;

public class ObjectStore(string path) : NosqliteStore(path)
{
    private readonly ConcurrentDictionary<string, dynamic> collections = [];
    public Collection<T>? Get<T>()
    {
        collections.TryGetValue(typeof(T).Name, out var collection);
        return collection;
    }

    public Collection<T> GetRequired<T>()
    {
        return Get<T>() ?? throw new CollectionNotFoundException();
    }

    public Collection? Get(string collection)
    {
        collections.TryGetValue(collection, out var result);
        return result;
    }

    public Collection GetRequired(string collection)
    {
        return Get(collection) ?? throw new CollectionNotFoundException();
    }

    public Collection<T> GetOrCreate<T>()
    {
        return collections.GetOrAdd(typeof(T).Name, (name) => new Collection<T>(this));
    }

    public Collection GetOrCreate(string collection)
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