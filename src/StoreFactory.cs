using VoidNone.NoSQLite.Internal;

namespace VoidNone.NoSQLite;

public class StoreFactory : IStoreFactory
{
    public IObjectStore CreateObjectStore(string path)
    {
        return new ObjectStore(path);
    }

    public IKeyValueStore CreateKeyValueStore(string path)
    {
        return new KeyValueStore(path);
    }
}