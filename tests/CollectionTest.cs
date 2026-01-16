using VoidNone.NoSQLiteTest.Models;
using VoidNone.NoSQLite;

namespace VoidNone.NoSQLiteTest;

[TestClass]
public class CollectionTest
{
    [TestMethod]
    public async Task AddAsync()
    {
        var db = Database.Create();
        var users = db.GetOrCreateCollection<User>();
        await users.AddAsync(new User
        {
            Name = "alex"
        });
        Assert.AreEqual(1, users.Query.Count());
        Assert.AreEqual("alex", (await users.Query.TakeAsync()).First().Data.Name);
    }

    [TestMethod]
    public async Task Exists()
    {
        var db = Database.Create();
        var users = db.GetOrCreateCollection<User>();
        var user = await users.AddAsync(new User
        {
            Name = "alex"
        });
        Assert.IsTrue(users.Exists(user.Id));
    }

    [TestMethod]
    public async Task GetByIdAsync()
    {
        var db = Database.Create();
        var users = db.GetOrCreateCollection<User>();
        var user = await users.AddAsync(new User
        {
            Name = "alex"
        });
        var docInDb = await users.GetByIdAsync(user.Id);
        Assert.IsNotNull(docInDb);
        Assert.AreEqual("alex", user.Data.Name);
    }

    [TestMethod]
    public async Task GetByOwnerIdAsync()
    {
        var db = Database.Create();
        var posts = db.GetOrCreateCollection<Post>();

        for (int i = 0; i < 2; i++)
        {
            await posts.AddAsync(new Post
            {
                Title = "Hello world"
            }, new NewDocumentOptions
            {
                OwnerId = "id1"
            });
        }

        var list = await posts.GetByOwnerIdAsync("id1");
        Assert.HasCount(2, list);
    }
}