using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace VoidNone.NoSQLite.Internal;

internal static class SqliteDataReaderExtensions
{
    extension(SqliteDataReader reader)
    {
        internal Document<T> ReadDocument<T>()
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
                Data = JsonSerializer.Deserialize<T>(reader.GetStream(7), JsonSerializerOptions.Database) ?? throw new DocumentDataInvalidException(),
            };
        }
    }
}