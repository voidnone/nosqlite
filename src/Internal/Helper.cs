namespace VoidNone.NoSQLite.Internal;

public static class Helper
{
    public static DateTimeOffset UnixTimeMilliseconds()
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
    }
}