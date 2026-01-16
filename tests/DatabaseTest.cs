using VoidNone.NoSQLite;
using VoidNone.NoSQLiteTest.Models;

namespace VoidNone.NoSQLiteTest;

[TestClass]
public class DatabaseTest
{
    [TestCleanup]
    public void TestCleanup()
    {
        Database.Remove("test.db");
    }

    [TestMethod]
    public void InMemory()
    {
        var db = Database.Create();
        Assert.IsTrue(db.Connection.InMemory);
        db = Database.Create("test.db");
        Assert.IsFalse(db.Connection.InMemory);
    }

    [TestMethod]
    public void GetCollection()
    {
        var db = Database.Create();
        var users = db.GetCollection(nameof(User));
        Assert.IsNull(users);
        db.GetOrCreateCollection(nameof(User));
        users = db.GetCollection(nameof(User));
        Assert.IsNotNull(users);
    }

    [TestMethod]
    public void GetCollectionT()
    {
        var db = Database.Create();
        var users = db.GetCollection<User>();
        Assert.IsNull(users);
        db.GetOrCreateCollection<User>();
        users = db.GetCollection<User>();
        Assert.IsNotNull(users);
    }

    [TestMethod]
    public void GetOrCreateCollection()
    {
        var db = Database.Create();
        var users = db.GetOrCreateCollection(nameof(User));
        Assert.IsNotNull(users);
    }

    [TestMethod]
    public void GetOrCreateCollectionT()
    {
        var db = Database.Create();
        var users = db.GetOrCreateCollection<User>();
        Assert.IsNotNull(users);
    }

    [TestMethod]
    public void GetRequiredCollection()
    {
        var db = Database.Create();

        Assert.ThrowsExactly<CollectionNotFoundException>(() =>
        {
            db.GetRequiredCollection(nameof(User));
        });

        db.GetOrCreateCollection(nameof(User));
        var users = db.GetRequiredCollection(nameof(User));
        Assert.IsNotNull(users);
    }

    [TestMethod]
    public void GetCollectionRequiredT()
    {
        var db = Database.Create();

        Assert.ThrowsExactly<CollectionNotFoundException>(() =>
        {
            db.GetRequiredCollection<User>();
        });

        db.GetOrCreateCollection<User>();
        var users = db.GetRequiredCollection<User>();
        Assert.IsNotNull(users);
    }

    [TestMethod]
    public void RemoveCollection()
    {
        var db = Database.Create();
        var success = db.RemoveCollection(nameof(User));
        Assert.IsFalse(success);
        db.GetOrCreateCollection(nameof(User));
        success = db.RemoveCollection(nameof(User));
        Assert.IsTrue(success);
    }

    [TestMethod]
    public void RemoveCollectionT()
    {
        var db = Database.Create();
        var success = db.RemoveCollection<User>();
        Assert.IsFalse(success);
        db.GetOrCreateCollection<User>();
        success = db.RemoveCollection<User>();
        Assert.IsTrue(success);
    }

    // [TestMethod]
    // public void Query()
    // {
    //     var db = Database.Create();
    // }
}