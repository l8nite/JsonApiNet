using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonApiNet.Helpers
{
    public static class TypeExtensions
    {
        // given some sort of IEnumerable<T>, return T
        // given any other Type, return it
        public static Type GetSingleElementType(this Type type)
        {
            return type.IsAssignableToGenericType(typeof(IEnumerable<>)) ? type.GetGenericArguments()[0] : type;
        }

        // http://stackoverflow.com/questions/74616/how-to-detect-if-type-is-another-generic-type/1075059#1075059
        public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            if (interfaceTypes.Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType))
            {
                return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            var baseType = givenType.BaseType;
            return baseType != null && IsAssignableToGenericType(baseType, genericType);
        }
    }
}