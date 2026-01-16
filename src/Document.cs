using System.ComponentModel.DataAnnotations;

namespace VoidNone.NoSQLite;

public class NewDocumentOptions
{
    public string? Id { get; set; }
    public string? OwnerId { get; set; }
    public bool Enabled { get; set; } = true;
    public string? Note { get; set; }
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