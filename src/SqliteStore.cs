using Microsoft.Data.Sqlite;

namespace VoidNone.Nosqlite;

public abstract class SqliteStore(string path)
{
    private string? connectionString;
    private bool initialized = false;

    protected int Execute(string sql, IDictionary<string, object>? parameters = null)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        if (parameters != null)
        {
            foreach (var item in parameters)
            {
                command.Parameters.AddWithValue(item.Key, item.Value);
            }
        }
        return command.ExecuteNonQuery();
    }

    private void Initialize()
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
        Execute("PRAGMA journal_mode = 'wal'");
    }

    public SqliteConnection OpenConnection()
    {
        if (initialized == false)
        {
            lock (this)
            {
                if (initialized == false)
                {
                    initialized = true;
                    Initialize();
                }
            }
        }

        var connection = new SqliteConnection(connectionString);
        connection.Open();
        return connection;
    }
}