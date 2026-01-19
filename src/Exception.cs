namespace VoidNone.NoSQLite;

public class CollectionNotFoundException(string name) : Exception($"Collection '{name}' not found")
{
}

public class DocumentNotFoundException(string id) : Exception($"Document with id '{id}' was not found")
{

}

public class DocumentDataInvalidException : Exception
{

}