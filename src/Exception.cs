namespace VoidNone.NoSQLite;

public class CollectionNotFoundException(string name) : Exception($"Collection '{name}' not found")
{
}

public class DocumentNotFoundException : Exception
{
    public DocumentNotFoundException()
    {
    }

    public DocumentNotFoundException(string id) : base($"Document with id '{id}' was not found")
    {
    }
}

public class DocumentDataInvalidException : Exception
{

}