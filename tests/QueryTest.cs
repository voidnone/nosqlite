using VoidNone.NoSQLiteTest.Models;
using VoidNone.NoSQLite;

namespace VoidNone.NoSQLiteTest;

[TestClass]
public class QueryTest
{
    [TestMethod]
    public async Task TakeAsync()
    {
        var db = await GetDatabaseAsync();
        var collection = db.GetOrCreateCollection<User>();
        var users = await collection.Query.Where("$.Name", "Alex").TakeAsync();
        Assert.AreEqual(1, users.Count());
    }

    [TestMethod]
    public async Task ExcludeAsync()
    {
        var db = await GetDatabaseAsync();
        var collection = db.GetOrCreateCollection<User>();
        var user = await collection.Query.Where("$.Name", "Alex").Exclude("$.Tags").FirstOrDefaultAsync();
        Assert.IsNull(user!.Data.Tags);
    }

    [TestMethod]
    public async Task ParentInAsync()
    {
        var db = await GetDatabaseAsync();
        var collection = db.GetOrCreateCollection<User>();
        var user = await collection.Query.FirstOrDefaultAsync();
        var posts = await db.GetOrCreateCollection<Post>().Query.OwnerIn(user!.Id).TakeAsync();
        Assert.AreEqual(1, posts.Count());
    }

    private async Task<Database> GetDatabaseAsync()
    {
        var db = Database.Create();
        var userCollection = db.GetOrCreateCollection<User>();
        var postCollection = db.GetOrCreateCollection<Post>();

        var user1 = await userCollection.AddAsync(new User
        {
            Name = "Alex",
            Age = 23,
            Tags = ["1", "2"]
        });

        await userCollection.AddAsync(new User
        {
            Name = "Jobs",
            Age = 24,
            Tags = ["1", "2"]
        });

        await postCollection.AddAsync(new Post
        {
            Title = "hello"
        }, new NewDocumentOptions
        {
            OwnerId = user1.Id
        }
        );

        await postCollection.AddAsync(new Post
        {
            Title = "world"
        });

        return db;
    }
}