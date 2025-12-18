using System.Collections.Concurrent;
using Microsoft.Data.Sqlite;

namespace VoidNone.Nosqlite;

public class NosqliteStore
{
    private readonly string path;
    private readonly string connectionString;
    private readonly ConcurrentDictionary<string, dynamic> collections = [];

    public NosqliteStore(string path)
    {
        this.path = path;
        var dir = Path.GetDirectoryName(path);

        if (!string.IsNullOrWhiteSpace(dir))
        {
            dir = Path.GetFullPath(dir);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        }

        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = path,
            Cache = SqliteCacheMode.Shared,
            Pooling = true,
        };

        connectionString = builder.ToString();
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA journal_mode = 'wal'";
        command.ExecuteNonQuery();
    }

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
        return collections.GetOrAdd(typeof(T).Name, (name) => new Collection<T>(connectionString));
    }

    public Collection GetOrCreate(string collection)
    {
        return collections.GetOrAdd(collection, (name) => new Collection(connectionString, name));
    }

    public void Remove<T>() => Remove(typeof(T).Name);

    public void Remove(string collection)
    {
        if (!collections.TryRemove(collection, out var _)) return;
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"DROP TABLE IF EXISTS `{collection}`";
        command.ExecuteNonQuery();
    }

    public void Remove() => File.Delete(path);
}