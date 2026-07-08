namespace VoidNone.NoSQLiteTest.Models;

public class Timestamp
{
    public DateTime DateTime { get; set; }
    public DateTime? NullableDateTime { get; set; }
    public DateTimeOffset DateTimeOffset { get; set; }
    public DateTimeOffset? NullableDateTimeOffset { get; set; }
    public TimeSpan TimeSpan { get; set; }
    public TimeSpan? NullableTimeSpan { get; set; }
}