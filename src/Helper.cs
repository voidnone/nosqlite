namespace VoidNone.NoSQLite;

public static class Helper
{
    public static DateTimeOffset UnixTimeMilliseconds()
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
    }
}