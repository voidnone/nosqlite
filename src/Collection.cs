using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace VoidNone.NoSQLite;

public class Collection<T>
{
    private readonly Connection connection;

    public Query<T> Query => new(connection, Name);

    internal Collection(Connection connection)
    {
        this.connection = connection;
        CreateTable();
    }

    public virtual string Name { get; init; } = typeof(T).Name;

    public async Task<Document<T>> GetRequiredByIdAsync(string id, CancellationToken token = default)
    {
        var result = await GetByIdAsync(id, token) ?? throw new DataCanNotBeNullException();
        return result;
    }

    public async Task<Document<T>?> GetByIdAsync(string id, CancellationToken token = default)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@Id", id}
        };

        using var reader = connection.Query($"""
        SELECT 
            rowid as Rowid,
            Id,
            OwnerId,
            CreationTime,
            LastWriteTime,
            Enabled,
            Note,
            json(Data) as Data
        FROM `{Name}` WHERE Id=@Id LIMIT 1
        """, parameters);
        var hasValue = reader.Read();
        if (!hasValue) return null;
        return await Collection<T>.ReadDocumentAsync(reader, token);
    }

    public async Task<Document<T>[]> GetByOwnerIdAsync(string ownerId, CancellationToken token = default)
    {
        using var dbConnection = connection.OpenConnection();
        using var command = dbConnection.CreateCommand();

        command.CommandText = $"""
        SELECT 
            rowid as Rowid,
            Id,
            OwnerId,
            CreationTime,
            LastWriteTime,
            Enabled,
            Note,
            json(Data) as Data 
        FROM `{Name}` WHERE OwnerId=@OwnerId
        """;

        command.Parameters.AddWithValue("@OwnerId", ownerId);
        using var reader = command.ExecuteReader();
        var result = new List<Document<T>>();

        while (reader.Read())
        {
            result.Add(await Collection<T>.ReadDocumentAsync(reader, token));
        }

        return [.. result];
    }

    public async Task<Document<T>> AddAsync(NewDocument<T> document, CancellationToken token = default)
    {
        if (document.Data is null)
        {
            throw new DocumentNotFoundException();
        }

        using var dbConnection = connection.OpenConnection();
        using var command = dbConnection.CreateCommand();
        command.CommandText = $"""
        INSERT INTO `{Name}` (
            Id,
            OwnerId,
            CreationTime,
            LastWriteTime,
            Data,
            Enabled,
            Note
        ) VALUES (
            @Id, 
            @OwnerId,
            @CreationTime,
            @LastWriteTime,
            jsonb(@Data),
            @Enabled,
            @Note
        )
        RETURNING 
            rowid as Rowid,
            Id,
            OwnerId,
            CreationTime,
            LastWriteTime,
            Enabled,
            Note,
            json(Data) as Data 
        """;

        var dataStream = new MemoryStream();
        await JsonSerializer.SerializeAsync(dataStream, document.Data, JsonSerializerOptions.Default, token);
        command.Parameters.AddWithValue("@Id", document.Id);
        command.Parameters.AddWithValue("@OwnerId", document.OwnerId);
        command.Parameters.AddWithValue("@CreationTime", document.CreationTime);
        command.Parameters.AddWithValue("@LastWriteTime", document.LastWriteTime);
        command.Parameters.AddWithValue("@Data", dataStream.ToArray());
        command.Parameters.AddWithValue("@Enabled", document.Enabled);
        command.Parameters.AddWithValue("@Note", document.Note);

        using var reader = command.ExecuteReader();
        reader.Read();
        return await Collection<T>.ReadDocumentAsync(reader, token);
    }

    public async Task<Document<T>> UpdateAsync(Document<T> document, CancellationToken token = default)
    {
        if (document.Data is null)
        {
            throw new DocumentNotFoundException();
        }

        using var dbConnection = connection.OpenConnection();
        using var command = dbConnection.CreateCommand();

        command.CommandText = $"""
        UPDATE `{Name}` 
        SET Data = jsonb(@Data),
            OwnerId = @OwnerId,
            Enabled = @Enabled,
            Note = @Note
        WHERE
            Id = @Id
        RETURNING 
            rowid as Rowid,
            Id,
            OwnerId,
            CreationTime,
            LastWriteTime,
            Enabled,
            Note,
            json(Data) as Data 
        """;

        var dataStream = new MemoryStream();
        await JsonSerializer.SerializeAsync(dataStream, document.Data, JsonSerializerOptions.Default, token);
        command.Parameters.AddWithValue("@Id", document.Id);
        command.Parameters.AddWithValue("@OwnerId", document.OwnerId);
        command.Parameters.AddWithValue("@Data", dataStream.ToArray());
        command.Parameters.AddWithValue("@Enabled", document.Enabled);
        command.Parameters.AddWithValue("@Note", document.Note);

        using var reader = command.ExecuteReader();
        reader.Read();
        return await Collection<T>.ReadDocumentAsync(reader, token);
    }

    public bool Exists(string id)
    {
        using var dbConnection = connection.OpenConnection();
        using var command = dbConnection.CreateCommand();
        command.CommandText = $"SELECT 1 FROM `{Name}` WHERE Id=@Id LIMIT 1";
        command.Parameters.AddWithValue("@Id", id);
        using var reader = command.ExecuteReader();
        return reader.Read();
    }

    public void Remove(string id) => Remove([id]);

    public void Remove(string[] ids)
    {
        using var dbConnection = connection.OpenConnection();
        using var command = dbConnection.CreateCommand();
        var idParameters = new List<string>();

        for (int i = 0; i < ids.Length; i++)
        {
            var name = $"@Id{i}";
            idParameters.Add(name);
            command.Parameters.AddWithValue(name, ids[i]);
        }

        command.CommandText = $"DELETE FROM `{Name}`WHERE Id IN ({string.Join(',', idParameters)})";
        command.ExecuteNonQuery();
    }

    private void CreateTable()
    {
        using var dbConnection = connection.OpenConnection();
        using var command = dbConnection.CreateCommand();

        command.CommandText = $"""
        CREATE TABLE IF NOT EXISTS `{Name}` (
            Id TEXT PRIMARY KEY,
            OwnerId TEXT NOT NULL,
            CreationTime INTEGER NOT NULL,
            LastWriteTime INTEGER NOT NULL,
            Data BLOB NOT NULL CHECK(json_valid(Data,8)),
            Enabled INTEGER NOT NULL CHECK(Enabled IS TRUE OR Enabled IS FALSE),
            Note TEXT NOT NULL
        );

        CREATE TRIGGER IF NOT EXISTS {Name}_TRIGGER
        AFTER UPDATE ON `{Name}`
        FOR EACH ROW
        BEGIN
            UPDATE `{Name}`
            SET LastWriteTime = unixepoch('subsec') * 1000
            WHERE Id = OLD.Id;
        END;

        CREATE INDEX IF NOT EXISTS {Name}_OwnerId_INDEX
        ON `{Name}`(OwnerId);

        CREATE INDEX IF NOT EXISTS {Name}_CreationTime_INDEX
        ON `{Name}`(CreationTime);

        CREATE INDEX IF NOT EXISTS {Name}_LastWriteTime_INDEX
        ON `{Name}`(LastWriteTime);

        CREATE INDEX IF NOT EXISTS {Name}_Enabled_INDEX
        ON `{Name}`(Enabled);
        """;

        command.ExecuteNonQuery();
    }

    internal static async Task<Document<T>> ReadDocumentAsync(SqliteDataReader reader, CancellationToken token)
    {
        return new Document<T>
        {
            RowId = reader.GetInt64(0),
            Id = reader.GetString(1),
            OwnerId = reader.GetString(2),
            CreationTime = DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64(3)),
            LastWriteTime = DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64(4)),
            Enabled = reader.GetBoolean(5),
            Note = reader.GetString(6),
            Data = await JsonSerializer.DeserializeAsync<T>(reader.GetStream(7), JsonSerializerOptions.Default, token) ?? throw new DocumentDataInvalidException(),
        };
    }
}

public class Collection(Connection connection, string name) : Collection<IDictionary<string, object>>(connection)
{
    public override string Name => name;
}