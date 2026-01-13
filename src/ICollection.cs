namespace VoidNone.NoSQLite;

public interface ICollection<T>
{
    IQuery<T> Query { get; }
    string Name { get; init; }
    Task<Document<T>> AddAsync(NewDocument<T> document, CancellationToken token = default);
    bool Exists(string id);
    Task<Document<T>?> GetByIdAsync(string id, CancellationToken token = default);
    Task<Document<T>[]> GetByOwnerIdAsync(string ownerId, CancellationToken token = default);
    Task<Document<T>> GetRequiredByIdAsync(string id, CancellationToken token = default);
    void Remove(string id);
    void Remove(string[] ids);
    Task<Document<T>> UpdateAsync(Document<T> document, CancellationToken token = default);
}

public interface ICollection : ICollection<IDictionary<string, object>>
{

}

