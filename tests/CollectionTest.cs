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

    [TestMethod]
    public async Task UpdateAsync()
    {
        var db = Database.Create();
        var posts = db.GetOrCreateCollection<Post>();
        var post = await posts.AddAsync(new Post
        {
            Title = "Hello world"
        });

        post.Data.Title = "Hello";
        post.Enabled = false;
        post.Note = "world";
        post.OwnerId = "123";

        post = await posts.UpdateAsync(post);
        Assert.AreEqual("Hello", post.Data.Title);
        Assert.IsFalse(post.Enabled);
        Assert.AreEqual("world", post.Note);
        Assert.AreEqual("123", post.OwnerId);
    }

    [TestMethod]
    public async Task Remove()
    {
        var db = Database.Create();
        var posts = db.GetOrCreateCollection<Post>();
        var post = await posts.AddAsync(new Post
        {
            Title = "Hello world"
        });

        Assert.IsTrue(posts.Exists(post.Id));
        posts.Remove(post.Id);
        Assert.IsFalse(posts.Exists(post.Id));
    }
}