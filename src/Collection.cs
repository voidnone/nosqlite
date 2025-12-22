namespace VoidNone.Nosqlite;

public class Collection(ObjectStore store, string name) : Collection<IDictionary<string, object>>(store)
{
    public override string Name => name;
}