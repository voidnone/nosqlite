namespace VoidNone.NoSQLite;


public abstract class NoSQLiteException : Exception
{
    public NoSQLiteException()
    {
    }

    public NoSQLiteException(string? message) : base(message)
    {
    }
}

public class NameInvalidException(string name) : NoSQLiteException($"Name '{name}' invalid")
{
}

public class CollectionNotFoundException(string name) : NoSQLiteException($"Collection with name '{name}' was not found")
{
}

public class DocumentNotFoundException : NoSQLiteException
{
    public DocumentNotFoundException() : base($"Document not found")
    {
    }

    public DocumentNotFoundException(string id) : base($"Document with id '{id}' was not found")
    {
    }
}

public class DocumentDataInvalidException : NoSQLiteException
{

}