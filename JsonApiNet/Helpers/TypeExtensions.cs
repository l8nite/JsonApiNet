using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonApiNet.Helpers
{
    public static class TypeExtensions
    {
        // type inherits from IList<T>, return true
        public static bool IsListType(this Type type)
        {
            return type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>));
        }

        // type is T or List<T>, return T
        public static Type GetSingleElementType(this Type type)
        {
            return type.IsListType() ? type.GetGenericArguments()[0] : type;
        }

        // type is Foo<T>, return T
        public static Type GetSingleGenericType(this Type type)
        {
            return type.IsGenericType ? type.GetGenericArguments()[0] : null;
        }
    }
}