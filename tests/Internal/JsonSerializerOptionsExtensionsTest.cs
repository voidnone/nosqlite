using System.Text.Json;
using VoidNone.NoSQLite;
using VoidNone.NoSQLiteTest.Models;

namespace VoidNone.NoSQLiteTest;

[TestClass]
public class JsonSerializerOptionsExtensionsTest
{
    [TestMethod]
    public void DateTime()
    {
        var db = Database.Create();
        var collection = db.GetOrCreateCollection<Timestamp>();
        collection.Add(new Timestamp
        {
            DateTime = System.DateTime.Today
        });
        var reader = db.Connection.Query("SELECT json(Data) FROM Timestamp");
        reader.Read();
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(reader.GetStream(0));
        var timestamp = jsonElement.GetProperty("dateTime").GetInt64();
        Assert.AreEqual(new DateTimeOffset(System.DateTime.Today).ToUnixTimeMilliseconds(), timestamp);
    }
}