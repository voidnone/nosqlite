namespace VoidNone.Nosqlite;

public class CollectionNotFoundException(string name) : Exception($"Collection '{name}' not found")
{
}

public class DataCanNotBeNullException : Exception
{

}

public class DocumentNotFoundException : Exception
{

}

public class DocumentDataInvalidException : Exception
{

}