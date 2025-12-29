namespace VoidNone.NoSQLite;


public interface IStoreFactory
{
    IKeyValueStore CreateKeyValueStore(string path);
    IObjectStore CreateObjectStore(string path);
}