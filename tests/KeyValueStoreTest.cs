using VoidNone.NoSQLite;

namespace VoidNone.NoSQLiteTest;

[TestClass]
public class KeyValueStoreTest
{
    private string tempFile = string.Empty;

    [TestInitialize]
    public void Init()
    {
        tempFile = Path.Combine(Path.GetTempPath(), "nosqlite_test_" + Guid.NewGuid() + ".db");
    }

    [TestCleanup]
    public void Cleanup()
    {
        try
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
        catch { }
    }

    [TestMethod]
    public void SetAndGet_ReturnsValue()
    {
        var store = new StoreFactory().CreateKeyValueStore(tempFile);
        store.Set("k1", "v1");
        var got = store.Get("k1");
        Assert.AreEqual("v1", got);
    }

    [TestMethod]
    public void Get_NonExistentKey_ReturnsNull()
    {
        var store = new StoreFactory().CreateKeyValueStore(tempFile);
        var got = store.Get("missing_key");
        Assert.IsNull(got);
    }

    [TestMethod]
    public void Set_OverwriteValue_UpdatesValue()
    {
        var store = new StoreFactory().CreateKeyValueStore(tempFile);
        store.Set("k2", "v2");
        store.Set("k2", "v2b");
        var got = store.Get("k2");
        Assert.AreEqual("v2b", got);
    }
}
