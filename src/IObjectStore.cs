namespace VoidNone.NoSQLite;

public interface IObjectStore
{
    ICollection<T>? Get<T>();
    ICollection? Get(string collection);
    ICollection<T> GetOrCreate<T>();
    ICollection GetOrCreate(string collection);
    ICollection<T> GetRequired<T>();
    ICollection GetRequired(string collection);
    void Remove<T>();
    void Remove(string collection);
    void Remove();
}