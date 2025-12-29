using System.Threading.Tasks;
using VoidNone.NoSQLite;

namespace VoidNone.NoSQLiteTest;

[TestClass]
public class CollectionTest
{
    [TestMethod]
    public async Task Document_CRUD_Async()
    {
        var store = new StoreFactory().CreateObjectStore("test.db");
        var userCollection = store.GetOrCreate<User>();

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
        Assert.AreEqual(doc.CreationTime, docOnDb.CreationTime);
        Assert.AreEqual(doc.LastWriteTime, docOnDb.LastWriteTime);
        Assert.AreEqual(doc.Data.Name, docOnDb.Data.Name);
        Assert.AreEqual(doc.Data.Age, docOnDb.Data.Age);
        Assert.AreEqual(doc.Data.Tags[0], docOnDb.Data.Tags![0]);
        Assert.AreEqual(doc.Data.Tags[1], docOnDb.Data.Tags[1]);
        docOnDb.Data.Name = "jobs";
        await userCollection.UpdateAsync(docOnDb);
        docOnDb = await userCollection.GetRequiredByIdAsync(doc.Id);
        Assert.AreEqual("jobs", docOnDb.Data.Name);
        Assert.IsTrue(docOnDb.LastWriteTime >= doc.LastWriteTime);
        userCollection.Remove(doc.Id);
        Assert.IsNull(await userCollection.GetByIdAsync(doc.Id));
    }

    [TestMethod]
    public async Task Document_ExistsAsync()
    {
        var store = new StoreFactory().CreateObjectStore("test.db");
        var userCollection = store.GetOrCreate<User>();

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