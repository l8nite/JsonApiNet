using System;

namespace JsonApiNet.Resolvers
{
    public interface IJsonApiTypeResolver
    {
        Type ResolveType(string typeName);
    }
}