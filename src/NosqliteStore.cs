using Microsoft.Data.Sqlite;

namespace VoidNone.Nosqlite;

public abstract class NosqliteStore
{
    private readonly string connectionString;

    public string ConnectionString => connectionString;

    public NosqliteStore(string path)
    {
        var dir = Path.GetDirectoryName(path);

        if (!string.IsNullOrWhiteSpace(dir))
        {
            dir = Path.GetFullPath(dir);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        }

        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = path,
            Cache = SqliteCacheMode.Shared,
            Pooling = true,
        };

        connectionString = builder.ToString();
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA journal_mode = 'wal'";
        command.ExecuteNonQuery();
    }

    protected int Execute(string sql)
    {
        using var connection = GetConnection();
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        return command.ExecuteNonQuery();
    }

    private SqliteConnection GetConnection()
    {
        var result = new SqliteConnection(ConnectionString);
        result.Open();
        return result;
    }
}