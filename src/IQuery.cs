namespace VoidNone.NoSQLite;

public interface IQuery<T>
{
    static abstract (string value, bool isNull) GetValue(object value);
    long Count();
    IQuery<T> Exclude(params string[] selectors);
    Task<Document<T>?> FirstOrDefaultAsync();
    IQuery<T> Order(string selector);
    IQuery<T> OrderByDescending(string selector);
    IQuery<T> OrWhere(string selector, object value);
    IQuery<T> OrWhere(string selector, Comparison comparison, object value);
    IQuery<T> OwnerIn(params string[] ids);
    IQuery<T> Skip(long count);
    Task<IEnumerable<Document<T>>> TakeAsync(CancellationToken token);
    Task<IEnumerable<Document<T>>> TakeAsync(long? count = null, CancellationToken token = default);
    IQuery<T> Where(string selector, object value);
    IQuery<T> Where(string selector, Comparison comparison, object value);
}