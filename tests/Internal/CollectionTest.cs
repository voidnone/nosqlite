using VoidNone.NoSQLiteTest.Models;
using VoidNone.NoSQLite;

namespace VoidNone.NoSQLiteTest.Internal;

[TestClass]
public class CollectionTest
{
    [TestMethod]
    public async Task Document_CRUD_Async()
    {
        var db = IDatabase.Create("test.db");
        var userCollection = db.GetOrCreateCollection<User>();

        var doc = new NewDocument<User>
        {
            Data = new User
            {
                Name = "alex",
                Age = 23,
                Tags = ["a", "b"]
            }
        };

        await userCollection.AddAsync(doc);
        var docOnDb = await userCollection.GetRequiredByIdAsync(doc.Id);
        Assert.AreEqual(doc.Id, docOnDb.Id);
        Assert.AreEqual(doc.CreationTime, docOnDb.CreationTime.ToUnixTimeMilliseconds());
        Assert.AreEqual(doc.LastWriteTime, docOnDb.LastWriteTime.ToUnixTimeMilliseconds());
        Assert.AreEqual(doc.Data.Name, docOnDb.Data.Name);
        Assert.AreEqual(doc.Data.Age, docOnDb.Data.Age);
        Assert.AreEqual(doc.Data.Tags[0], docOnDb.Data.Tags![0]);
        Assert.AreEqual(doc.Data.Tags[1], docOnDb.Data.Tags[1]);
        docOnDb.Data.Name = "jobs";
        await userCollection.UpdateAsync(docOnDb);
        docOnDb = await userCollection.GetRequiredByIdAsync(doc.Id);
        Assert.AreEqual("jobs", docOnDb.Data.Name);
        Assert.IsGreaterThanOrEqualTo(doc.LastWriteTime, docOnDb.LastWriteTime.ToUnixTimeMilliseconds());
        userCollection.Remove(doc.Id);
        Assert.IsNull(await userCollection.GetByIdAsync(doc.Id));
    }

    [TestMethod]
    public async Task Document_ExistsAsync()
    {
        var db = IDatabase.Create("test.db");
        var userCollection = db.GetOrCreateCollection<User>();

        var doc = new NewDocument<User>
        {
            Data = new User()
        };

        await userCollection.AddAsync(doc);
        var exist = userCollection.Exists(doc.Id);
        Assert.IsTrue(exist);
        userCollection.Remove(doc.Id);
        exist = userCollection.Exists(doc.Id);
        Assert.IsFalse(exist);
    }
}