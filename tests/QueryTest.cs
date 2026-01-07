using VoidNone.NoSQLite;

namespace VoidNone.NoSQLiteTest;

[TestClass]
public class QueryTest
{
    public required TestContext TestContext { get; set; }

    public IDatabase GetStore()
    {
        var path = TestContext.FullyQualifiedTestClassName + TestContext.TestName + ".db";
        return IDatabase.Create(path);
    }

    [TestInitialize]
    public async Task InitAsync()
    {
        var store = GetStore();
        var userCollection = store.GetOrCreate<User>();
        var postCollection = store.GetOrCreate<Post>();

        var user1 = await userCollection.AddAsync(new NewDocument<User>
        {
            Data = new User
            {
                Name = "Alex",
                Age = 23,
                Tags = ["1", "2"]
            }
        });

        await userCollection.AddAsync(new NewDocument<User>
        {
            Data = new User
            {
                Name = "Jobs",
                Age = 24,
                Tags = ["1", "2"]
            }
        });

        await postCollection.AddAsync(
            new NewDocument<Post>
            {
                Data = new Post
                {
                    Title = "hello"
                },
                OwnerId = user1.Id
            }
        );

        await postCollection.AddAsync(new NewDocument<Post>
        {
            Data = new Post
            {
                Title = "world"
            }
        });
    }

    [TestCleanup]
    public void Dispose()
    {
        var store = GetStore();
        store.Remove();
    }

    [TestMethod]
    public async Task TakeAsync()
    {
        var store = GetStore();
        var collection = store.GetOrCreate<User>();
        var users = await collection.Query.Where("$.Name", "Alex").TakeAsync();
        Assert.AreEqual(1, users.Count());
    }

    [TestMethod]
    public async Task ExcludeAsync()
    {
        var store = GetStore();
        var collection = store.GetOrCreate<User>();
        var user = await collection.Query.Where("$.Name", "Alex").Exclude("$.Tags").FirstOrDefaultAsync();
        Assert.IsNull(user!.Data.Tags);
    }

    [TestMethod]
    public async Task ParentInAsync()
    {
        var store = GetStore();
        var collection = store.GetOrCreate<User>();
        var user = await collection.Query.FirstOrDefaultAsync();
        var posts = await store.GetOrCreate<Post>().Query.OwnerIn(user!.Id).TakeAsync();
        Assert.AreEqual(1, posts.Count());
    }
}