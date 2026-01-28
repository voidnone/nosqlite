namespace VoidNone.NoSQLite.Internal;

internal static class TypeExtensions
{
    extension(Type type)
    {
        public string GetCollectionName()
        {
            if (!type.IsGenericType) return type.Name;
            var genericTypeName = type.Name;

            var tickIndex = genericTypeName.IndexOf('`');
            if (tickIndex > 0)
            {
                genericTypeName = genericTypeName[..tickIndex];
            }

            var genericArgNames = type.GetGenericArguments().Select(GetCollectionName);
            return genericTypeName + string.Concat(genericArgNames);
        }
    }
}