using VoidNone.NoSQLite;
using VoidNone.NoSQLiteTest.Models;

namespace VoidNone.NoSQLiteTest.Internal;

[TestClass]
public class DatabaseTest
{
    [TestMethod]
    public void GetCollection()
    {
        var db = IDatabase.Create();
        var users = db.GetCollection(nameof(User));
        Assert.IsNull(users);
        db.GetOrCreateCollection(nameof(User));
        users = db.GetCollection(nameof(User));
        Assert.IsNotNull(users);
    }

    [TestMethod]
    public void GetCollectionT()
    {
        var db = IDatabase.Create();
        var users = db.GetCollection<User>();
        Assert.IsNull(users);
        db.GetOrCreateCollection<User>();
        users = db.GetCollection<User>();
        Assert.IsNotNull(users);
    }

    [TestMethod]
    public void GetOrCreateCollection()
    {
        var db = IDatabase.Create();
        var users = db.GetOrCreateCollection(nameof(User));
        Assert.IsNotNull(users);
    }

    [TestMethod]
    public void GetOrCreateCollectionT()
    {
        var db = IDatabase.Create();
        var users = db.GetOrCreateCollection<User>();
        Assert.IsNotNull(users);
    }

    [TestMethod]
    public void GetCollectionRequired()
    {
        var db = IDatabase.Create();

        Assert.ThrowsExactly<CollectionNotFoundException>(() =>
        {
            db.GetCollectionRequired(nameof(User));
        });

        db.GetOrCreateCollection(nameof(User));
        var users = db.GetCollectionRequired(nameof(User));
        Assert.IsNotNull(users);
    }

    [TestMethod]
    public void GetCollectionRequiredT()
    {
        var db = IDatabase.Create();

        Assert.ThrowsExactly<CollectionNotFoundException>(() =>
        {
            db.GetCollectionRequired<User>();
        });

        db.GetOrCreateCollection<User>();
        var users = db.GetCollectionRequired<User>();
        Assert.IsNotNull(users);
    }

    [TestMethod]
    public void RemoveCollection()
    {
        var db = IDatabase.Create();
        var success = db.RemoveCollection(nameof(User));
        Assert.IsFalse(success);
        db.GetOrCreateCollection(nameof(User));
        success = db.RemoveCollection(nameof(User));
        Assert.IsTrue(success);
    }

    [TestMethod]
    public void RemoveCollectionT()
    {
        var db = IDatabase.Create();
        var success = db.RemoveCollection<User>();
        Assert.IsFalse(success);
        db.GetOrCreateCollection<User>();
        success = db.RemoveCollection<User>();
        Assert.IsTrue(success);
    }
}