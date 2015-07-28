using System;
using System.Reflection;

namespace JsonApiNet.Resolvers
{
    public interface IJsonApiPropertyResolver
    {
        PropertyInfo ResolveJsonApiAttribute(Type type, string attributeName);

        PropertyInfo ResolveJsonApiRelationship(Type type, string relationshipName);

        PropertyInfo ResolveJsonApiType(Type type);

        PropertyInfo ResolveJsonApiId(Type type);
    }
}