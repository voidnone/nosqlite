using VoidNone.NoSQLite;

namespace VoidNone.NoSQLiteTest;

[TestClass]
public class ConnectionTest
{
    [TestCleanup]
    public void TestCleanup()
    {
        Database.Remove("test.db");
    }

    [TestMethod]
    public void InMemory()
    {
        var connection = new Connection(null);
        Assert.IsTrue(connection.InMemory);
        connection = new Connection("test.db");
        Assert.IsFalse(connection.InMemory);
    }

    [TestMethod]
    public void Execute()
    {
        var connection = new Connection(null);
        var result = connection.Execute("CREATE TABLE Test (Id TEXT PRIMARY KEY)");
        Assert.AreEqual(0, result);
        result = connection.Execute("INSERT INTO Test (Id) VALUES (@Id)", new Dictionary<string, object> { { "@Id", "123" } });
        Assert.AreEqual(1, result);
    }
}