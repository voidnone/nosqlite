using System.Buffers;

namespace VoidNone.NoSQLite.Internal;

internal static class Validator
{
    private static readonly SearchValues<char> forbiddenChars = SearchValues.Create(" \t\n'\"`;/\\()[]");

    public static void ValidateName(string name)
    {
        var valid = name.IndexOfAny(forbiddenChars) == -1;
        if (!valid) throw new NameInvalidException(name);
    }
}
