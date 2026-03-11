using Microsoft.Data.Sqlite;

namespace VoidNone.NoSQLite;

public class Connection
{
    private string? connectionString;
    private volatile bool initialized = false;
    private readonly string path;
    private SqliteConnection? inMemoryConnection;

    public bool InMemory { get; }

    internal Connection(string? path)
    {
        this.path = Path.GetFullPath(path ?? (Guid.NewGuid().ToString() + ".db"));
        InMemory = path == null;
    }

    public int Execute(string sql, IDictionary<string, object>? parameters = null)
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

    internal SqliteDataReader Query(string sql, IDictionary<string, object>? parameters = null)
    {
        var connection = OpenConnection();
        var command = connection.CreateCommand();
        command.CommandText = sql;
        if (parameters != null)
        {
            foreach (var item in parameters)
            {
                command.Parameters.AddWithValue(item.Key, item.Value);
            }
        }

        return command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
    }

    private void Initialize()
    {
        if (!InMemory)
        {
            var dir = Path.GetDirectoryName(path);
            if (dir != null && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
        }

        connectionString = GetConnectionString();

        if (InMemory)
        {
            inMemoryConnection = new SqliteConnection(connectionString);
            inMemoryConnection.Open();
        }

        Execute("PRAGMA journal_mode = 'wal'");
    }

    private string GetConnectionString()
    {
        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = path,
            Cache = SqliteCacheMode.Shared,
            Pooling = true,
            Mode = InMemory ? SqliteOpenMode.Memory : SqliteOpenMode.ReadWriteCreate
        };

        return builder.ToString();
    }

    internal SqliteConnection OpenConnection()
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

    internal void Close()
    {
        if (InMemory) return;
        if (!File.Exists(path)) return;
        SqliteConnection.ClearPool(new SqliteConnection(GetConnectionString()));
    }
}