using VoidNone.NoSQLite;
using VoidNone.NoSQLiteTest.Models;

namespace VoidNone.NoSQLite.Tests;

[TestClass]
public class QueryTest
{
    [TestMethod]
    public void Take()
    {
        var db = Database.Create();
        var users = db.GetOrCreateCollection<User>();
        users.Add(new User { Name = "alex" });
        var result = users.Query.Take().ToArray();
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("alex", result[0].Data.Name);
        users.Add(new User { Name = "jobs" });

        result = users.Query.Take().ToArray();
        Assert.AreEqual(2, result.Count());
        Assert.AreEqual("jobs", result[1].Data.Name);
    }

    [TestMethod]
    public void Exclude()
    {
        var db = Database.Create();
        var collection = db.GetOrCreateCollection<User>();
        collection.Add(new User { Name = "alex", Tags = ["a", "b"] });
        var user = collection.Query.Exclude("$.tags").FirstOrDefault();
        Assert.IsNull(user!.Data.Tags);
    }

    [TestMethod]
    public void OwnerIn()
    {
        var db = Database.Create();
        var users = db.GetOrCreateCollection<User>();
        users.Add(new User { Name = "alex" }, new DocumentOptions
        {
            OwnerId = "123"
        });
        var posts = users.Query.OwnerIn("123").Take();
        Assert.AreEqual(1, posts.Count());
    }

    [TestMethod]
    public void WhereAsync()
    {
        var db = Database.Create();
        var users = db.GetOrCreateCollection<User>();
        users.Add(new User { Name = "alex" }, new DocumentOptions
        {
            OwnerId = "123"
        });
        var posts = users.Query.Where("$.name", "alex").Take();
        Assert.AreEqual(1, posts.Count());
    }
}