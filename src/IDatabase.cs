using VoidNone.NoSQLite.Internal;

namespace VoidNone.NoSQLite;

public interface IDatabase
{
    string? Path { get; }

    ICollection<T>? GetCollection<T>();
    ICollection? GetCollection(string name);
    ICollection<T> GetOrCreateCollection<T>();
    ICollection GetOrCreateCollection(string name);
    ICollection<T> GetCollectionRequired<T>();
    ICollection GetCollectionRequired(string name);
    bool RemoveCollection<T>();
    bool RemoveCollection(string name);

    static IDatabase Create(string? path = null)
    {
        return new Database(path);
    }

    static bool Remove(string path)
    {
        if (!File.Exists(path)) return false;
        File.Delete(path);
        return true;
    }
}