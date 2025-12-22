namespace VoidNone.Nosqlite;

public class KeyValue
{
    public required string Key { get; set; }
    public required string Value { get; set; }
    public required DateTimeOffset CreationTime { get; init; }
}