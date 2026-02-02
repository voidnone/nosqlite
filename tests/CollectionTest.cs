using VoidNone.NoSQLite;
using VoidNone.NoSQLiteTest.Models;

namespace VoidNone.NoSQLiteTest;

[TestClass]
public class CollectionTest
{

    [TestMethod]
    public async Task NameAsync()
    {
        var db = Database.Create();
        var users = db.GetOrCreateCollection<User>();
        var wrappers = db.GetOrCreateCollection<Wrapper<User>>();
        Assert.AreEqual("User", users.Name);
        Assert.AreEqual("WrapperUser", wrappers.Name);
    }

    [TestMethod]
    public async Task AddAsync()
    {
        var db = Database.Create();
        var users = db.GetOrCreateCollection<User>();
        var user = await users.AddAsync(new User
        {
            Name = "alex"
        });
        Assert.AreEqual(1, user.RowId);
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
        var id = Guid.NewGuid().ToString();
        var doc = await users.GetByIdAsync(id);
        Assert.IsNull(doc);
        var user = await users.AddAsync(new User
        {
            Name = "alex"
        }, new DocumentOptions
        {
            Id = id
        });
        doc = await users.GetByIdAsync(user.Id);
        Assert.IsNotNull(doc);
    }

    [TestMethod]
    public async Task GetRequiredByIdAsync()
    {
        var db = Database.Create();
        var users = db.GetOrCreateCollection<User>();
        var id = Guid.NewGuid().ToString();

        await Assert.ThrowsExactlyAsync<DocumentNotFoundException>(async () =>
        {
            await users.GetRequiredByIdAsync(id);
        });

        var user = await users.AddAsync(new User
        {
            Name = "alex"
        }, new DocumentOptions
        {
            Id = id
        });
        var doc = await users.GetRequiredByIdAsync(user.Id);
        Assert.IsNotNull(doc);
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
            }, new DocumentOptions
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
        await Task.Delay(TimeSpan.FromMilliseconds(1));
        var result = await posts.UpdateAsync(post);
        Assert.AreEqual(post.RowId, result.RowId);
        Assert.AreEqual(post.Id, result.Id);
        Assert.AreEqual(post.CreationTime, result.CreationTime);
        Assert.IsTrue(result.LastWriteTime > post.LastWriteTime);
        Assert.AreEqual("Hello", result.Data.Title);
        Assert.IsFalse(result.Enabled);
        Assert.AreEqual("world", result.Note);
        Assert.AreEqual("123", result.OwnerId);
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