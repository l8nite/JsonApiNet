using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Humanizer;
using JsonApiNet.Attributes;
using JsonApiNet.Exceptions;
using Microsoft.Extensions.DependencyModel;

namespace JsonApiNet.Resolvers
{
    internal class ReflectingTypeResolver : IJsonApiTypeResolver
    {
        private readonly Dictionary<string, Type> _typeByJsonApiResourceTypeName;
        private readonly Dictionary<string, Type> _typeByName;

        public ReflectingTypeResolver(Type rootType, Assembly[] additionalAssemblies = null)
        {
            _typeByJsonApiResourceTypeName = new Dictionary<string, Type>();
            _typeByName = new Dictionary<string, Type>();

            InitializeLookupTables(rootType, additionalAssemblies);
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

        private void InitializeLookupTables(Type rootType, Assembly[] additionalAssemblies)
        {
            // these are first-one-wins into the _typeByName cache, so they are resolved in top-down priority order
            if (rootType != null)
            {
                AddTypeToLookupTables(rootType);

                // add the rest of the types in the rootType's namespace, then everything else in its Assembly
                var leftovers = new List<Type>();
                foreach (var type in rootType.GetTypeInfo().Assembly.GetTypes())
                {
                    if (type.Namespace == rootType.Namespace)
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

            if(additionalAssemblies != null)
            {
                foreach (var assembly in additionalAssemblies)
                {
                    foreach(var exportedType in assembly.ExportedTypes)
                    {
                        AddTypeToLookupTables(exportedType);
                    }
                }
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
            var typeInfo = type.GetTypeInfo();

            if (!typeInfo.IsDefined(typeof(JsonApiResourceTypeAttribute), true))
            {
                return;
            }

            var attribute =
                typeInfo.GetCustomAttributes(typeof(JsonApiResourceTypeAttribute), true).FirstOrDefault() as JsonApiResourceTypeAttribute;

            if (attribute != null)
            {
                _typeByJsonApiResourceTypeName[attribute.JsonApiResourceTypeName] = type;
            }
        }
    }
}