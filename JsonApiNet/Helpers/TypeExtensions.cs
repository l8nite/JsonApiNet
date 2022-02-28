using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JsonApiNet.Helpers
{
    public static class TypeExtensions
    {
        // given some sort of IEnumerable<T>, return T
        // given any other Type, return it
        public static Type GetSingleElementType(this Type type)
        {
            return type.IsAssignableToGenericType(typeof(IEnumerable<>)) ? type.GenericTypeArguments[0] : type;
        }

        // http://stackoverflow.com/questions/74616/how-to-detect-if-type-is-another-generic-type/1075059#1075059
        public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            var givenTypeInfo = givenType.GetTypeInfo();
            var interfaceTypes = givenTypeInfo.GetInterfaces();

            if (interfaceTypes.Any(it => it.GetTypeInfo().IsGenericType && it.GetGenericTypeDefinition() == genericType))
            {
                return true;
            }

            if (givenTypeInfo.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            var baseType = givenTypeInfo.BaseType;
            return baseType != null && IsAssignableToGenericType(baseType, genericType);
        }
    }
}