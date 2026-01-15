using VoidNone.NoSQLiteTest.Models;
using VoidNone.NoSQLite;

namespace VoidNone.NoSQLiteTest.Internal;

[TestClass]
public class CollectionTest
{
    [TestMethod]
    public async Task AddAsync()
    {
        var db = IDatabase.Create();
        var users = db.GetOrCreateCollection<User>();
        var user = await users.AddAsync(new NewDocument<User>
        {
            Data = new User
            {
                Name = "alex"
            }
        });
        Assert.AreEqual(1, users.Query.Count());
        Assert.AreEqual("alex", (await users.Query.TakeAsync()).First().Data.Name);
    }

    [TestMethod]
    public async Task Exists()
    {
        var db = IDatabase.Create();
        var users = db.GetOrCreateCollection<User>();
        var doc = new NewDocument<User>
        {
            Data = new User
            {
                Name = "alex"
            }
        };
        Assert.IsFalse(users.Exists(doc.Id));
        await users.AddAsync(doc);
        Assert.IsTrue(users.Exists(doc.Id));
    }

    [TestMethod]
    public async Task GetByIdAsync()
    {
        var db = IDatabase.Create();
        var users = db.GetOrCreateCollection<User>();
        var doc = new NewDocument<User>
        {
            Data = new User
            {
                Name = "alex"
            }
        };
        Assert.IsNull(await users.GetByIdAsync(doc.Id));
        await users.AddAsync(doc);
        var docInDb = await users.GetByIdAsync(doc.Id);
        Assert.IsNotNull(docInDb);
        Assert.AreEqual("alex", doc.Data.Name);
    }
}