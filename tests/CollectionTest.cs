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
    public void Add()
    {
        var db = Database.Create();
        var users = db.GetOrCreateCollection<User>();
        var user = users.Add(new User
        {
            Name = "alex"
        });
        Assert.AreEqual(1, user.RowId);
        Assert.AreEqual(1, users.Query.Count());
        Assert.AreEqual("alex", users.Query.Take().First().Data.Name);
    }

    [TestMethod]
    public void Exists()
    {
        var db = Database.Create();
        var users = db.GetOrCreateCollection<User>();
        var user = users.Add(new User
        {
            Name = "alex"
        });
        Assert.IsTrue(users.Exists(user.Id));
    }

    [TestMethod]
    public void GetById()
    {
        var db = Database.Create();
        var users = db.GetOrCreateCollection<User>();
        var id = Guid.NewGuid().ToString();
        var doc = users.GetById(id);
        Assert.IsNull(doc);
        var user = users.Add(new User
        {
            Name = "alex"
        }, new DocumentOptions
        {
            Id = id
        });
        doc = users.GetById(user.Id);
        Assert.IsNotNull(doc);
    }

    [TestMethod]
    public void GetRequiredById()
    {
        var db = Database.Create();
        var users = db.GetOrCreateCollection<User>();
        var id = Guid.NewGuid().ToString();

        Assert.ThrowsExactly<DocumentNotFoundException>(() =>
       {
           users.GetRequiredById(id);
       });

        var user = users.Add(new User
        {
            Name = "alex"
        }, new DocumentOptions
        {
            Id = id
        });
        var doc = users.GetRequiredById(user.Id);
        Assert.IsNotNull(doc);
    }

    [TestMethod]
    public void GetByOwnerId()
    {
        var db = Database.Create();
        var posts = db.GetOrCreateCollection<Post>();

        for (int i = 0; i < 2; i++)
        {
            posts.Add(new Post
            {
                Title = "Hello world"
            }, new DocumentOptions
            {
                OwnerId = "id1"
            });
        }

        var list = posts.GetByOwnerId("id1");
        Assert.HasCount(2, list);
    }

    [TestMethod]
    public void Update()
    {
        var db = Database.Create();
        var posts = db.GetOrCreateCollection<Post>();
        var post = posts.Add(new Post
        {
            Title = "Hello world"
        });

        post.Data.Title = "Hello";
        post.Enabled = false;
        post.Note = "world";
        post.OwnerId = "123";
        Thread.Sleep(TimeSpan.FromMilliseconds(1));
        var result = posts.Update(post);
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
    public void Remove()
    {
        var db = Database.Create();
        var posts = db.GetOrCreateCollection<Post>();
        var post = posts.Add(new Post
        {
            Title = "Hello world"
        });

        Assert.IsTrue(posts.Exists(post.Id));
        posts.Remove(post.Id);
        Assert.IsFalse(posts.Exists(post.Id));
    }

    [TestMethod]
    public void EnsureIndex()
    {
        var db = Database.Create();
        var posts = db.GetOrCreateCollection<Post>();
        posts.EnsureIndex("Title");
        var post = posts.Add(new Post
        {
            Title = "Hello world"
        });

        Assert.IsTrue(posts.Exists(post.Id));
        posts.Remove(post.Id);
        Assert.IsFalse(posts.Exists(post.Id));
    }
}