using System.ComponentModel.DataAnnotations;

namespace VoidNone.NoSQLite;

public class NewDocument<T>
{
    [Range(1, 64)]
    public string Id { get; init; } = Guid.NewGuid().ToString();

    [Range(0, 64)]
    public string OwnerId { get; init; } = string.Empty;
    public DateTimeOffset CreationTime { get; init; } = Helper.UnixTimeMilliseconds();
    public DateTimeOffset LastWriteTime { get; init; } = Helper.UnixTimeMilliseconds();
    public required T Data { get; set; }
    public bool Enabled { get; set; } = true;
    public string Note { get; set; } = string.Empty;
}

public class Document<T>
{
    [Range(1, 64)]
    public required string Id { get; init; }
    [Range(0, 64)]
    public required string OwnerId { get; set; }
    public required long RowId { get; init; }
    public required DateTimeOffset CreationTime { get; init; }
    public required DateTimeOffset LastWriteTime { get; init; }
    public required T Data { get; set; }
    public required bool Enabled { get; set; } = true;
    public required string Note { get; set; }
}