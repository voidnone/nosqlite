using VoidNone.NoSQLite;
using VoidNone.NoSQLiteTest.Models;

namespace VoidNone.NoSQLiteTest;

[TestClass]
public class QueryTest
{
    [TestMethod]
    public async Task TakeAsync()
    {
        var db = Database.Create();
        var users = db.GetOrCreateCollection<User>();
        await users.AddAsync(new User { Name = "alex" });
        var result = await users.Query.TakeAsync();
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("alex", result[0].Data.Name);
        await users.AddAsync(new User { Name = "jobs" });
        result = await users.Query.TakeAsync();
        Assert.AreEqual(2, result.Count());
        Assert.AreEqual("jobs", result[1].Data.Name);
    }

    [TestMethod]
    public async Task ExcludeAsync()
    {
        var db = Database.Create();
        var collection = db.GetOrCreateCollection<User>();
        await collection.AddAsync(new User { Name = "alex", Tags = ["a", "b"] });
        var user = await collection.Query.Exclude("$.Tags").FirstOrDefaultAsync();
        Assert.IsNull(user!.Data.Tags);
    }

    [TestMethod]
    public async Task OwnerInAsync()
    {
        var db = Database.Create();
        var users = db.GetOrCreateCollection<User>();
        await users.AddAsync(new User { Name = "alex" }, new NewDocumentOptions
        {
            OwnerId = "123"
        });
        var posts = await users.Query.OwnerIn("123").TakeAsync();
        Assert.AreEqual(1, posts.Count());
    }
}