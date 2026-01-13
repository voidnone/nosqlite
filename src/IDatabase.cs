using VoidNone.NoSQLite.Internal;

namespace VoidNone.NoSQLite;

public interface IDatabase
{
    string Path { get; }

    ICollection<T>? GetCollection<T>();
    ICollection? GetCollection(string name);
    ICollection<T> GetOrCreateCollection<T>();
    ICollection GetOrCreateCollection(string name);
    ICollection<T> GetCollectionRequired<T>();
    ICollection GetCollectionRequired(string name);
    void RemoveCollection<T>();
    void RemoveCollection(string name);

    static IDatabase Create(string path)
    {
        return new Database(path);
    }

    static void Remove(string path)
    {
        if (!File.Exists(path)) return;
        File.Delete(path);
    }
}