using System;
using System.Collections.Generic;
using JsonApiNet.Exceptions;

namespace JsonApiNet.Resolvers
{
    public class StringToTypeResolver : IJsonApiTypeResolver
    {
        private readonly Dictionary<string, Type> _stringToType;

        public StringToTypeResolver()
            : this(null)
        {
        }

        public StringToTypeResolver(Dictionary<string, Type> stringToType)
        {
            _stringToType = stringToType ?? new Dictionary<string, Type>();
        }

        public void RegisterType(string typeName, Type type)
        {
            _stringToType[typeName] = type;
        }

        public void DeregisterType(string typeName)
        {
            if (_stringToType.ContainsKey(typeName))
            {
                _stringToType.Remove(typeName);
            }
        }

        public Type ResolveType(string typeName)
        {
            if (_stringToType.ContainsKey(typeName))
            {
                return _stringToType[typeName];
            }

            throw new JsonApiTypeNotFoundException(string.Format("No Type was registered for {0}", typeName));
        }
    }
}