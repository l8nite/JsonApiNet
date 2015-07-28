using System;
using System.Collections.Generic;
using System.Linq;
using Humanizer;
using JsonApiNet.Attributes;
using JsonApiNet.Exceptions;
using JsonApiNet.Helpers;

namespace JsonApiNet.Resolvers
{
    internal class ReflectingTypeResolver : IJsonApiTypeResolver
    {
        private readonly Type _protoType;
        private readonly Dictionary<string, Type> _typeByJsonApiResourceTypeName;
        private readonly Dictionary<string, Type> _typeByName;

        public ReflectingTypeResolver()
            : this(null)
        {
        }

        public ReflectingTypeResolver(Type protoType)
        {
            _protoType = protoType;

            _typeByJsonApiResourceTypeName = new Dictionary<string, Type>();
            _typeByName = new Dictionary<string, Type>();

            InitializeLookupTables();
        }

        public Type ResolveType(string typeName)
        {
            if (_typeByJsonApiResourceTypeName.ContainsKey(typeName))
            {
                return _typeByJsonApiResourceTypeName[typeName];
            }

            var classifiedName = Classify(typeName);

            if (_typeByName.ContainsKey(classifiedName))
            {
                return _typeByName[classifiedName];
            }

            throw new JsonApiTypeNotFoundException(string.Format("Could not ResolveType for '{0}'", typeName));
        }

        private static string Classify(string typeName)
        {
            return typeName.Underscore().Singularize().Pascalize();
        }

        private void InitializeLookupTables()
        {
            // these are first-one-wins into the _typeByName cache, so they are resolved in top-down priority order
            if (_protoType != null)
            {
                AddTypeToLookupTables(_protoType);

                // add the rest of the types in the _protoType's namespace, then everything else in its Assembly
                var leftovers = new List<Type>();
                foreach (var type in _protoType.Assembly.GetTypes())
                {
                    if (type.Namespace == _protoType.Namespace)
                    {
                        AddTypeToLookupTables(type);
                    }
                    else
                    {
                        leftovers.Add(type);
                    }
                }

                foreach (var type in leftovers)
                {
                    AddTypeToLookupTables(type);
                }
            }

            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()))
            {
                AddTypeToLookupTables(type);
            }
        }

        private void AddTypeToLookupTables(Type type)
        {
            if (type == null)
            {
                return;
            }

            // we've seen it already
            if (_typeByName.ContainsKey(type.Name))
            {
                return;
            }

            // look this Type up by name
            _typeByName[type.Name] = type;

            if (!type.IsDefined(typeof(JsonApiResourceTypeAttribute), true))
            {
                return;
            }

            var attribute =
                type.GetCustomAttributes(typeof(JsonApiResourceTypeAttribute), true).FirstOrDefault() as JsonApiResourceTypeAttribute;

            if (attribute != null)
            {
                _typeByJsonApiResourceTypeName[attribute.JsonApiResourceTypeName] = type;
            }
        }
    }
}