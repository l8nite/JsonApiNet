using System;
using System.Collections.Generic;
using System.Reflection;
using Humanizer;
using JsonApiNet.Attributes;
using JsonApiNet.Exceptions;

namespace JsonApiNet.Helpers
{
    internal class ResourcePropertyResolver
    {
        private readonly Dictionary<Type, PropertyInfo> _propertyInfoForAttributeType;
        private readonly Dictionary<string, PropertyInfo> _propertyInfoForJsonApiAttributeName;
        private readonly Dictionary<string, PropertyInfo> _propertyInfoForJsonApiRelationshipName;
        private readonly Dictionary<string, PropertyInfo> _propertyInfoForPropertyName;

        public Type ResourceType { get; private set; }

        public ResourcePropertyResolver(Type resourceType)
        {
            ResourceType = resourceType;

            _propertyInfoForPropertyName = new Dictionary<string, PropertyInfo>();
            _propertyInfoForJsonApiAttributeName = new Dictionary<string, PropertyInfo>();
            _propertyInfoForJsonApiRelationshipName = new Dictionary<string, PropertyInfo>();
            _propertyInfoForAttributeType = new Dictionary<Type, PropertyInfo>();


            InitializeLookupTables();
        }

        public PropertyInfo PropertyInfoForAttributeType(Type attributeType)
        {
            // Look for a property set via the [JsonApiId], etc. type
            if (_propertyInfoForAttributeType.ContainsKey(attributeType))
            {
                return _propertyInfoForAttributeType[attributeType];
            }

            var implicitName = ImplicitPropertyNameForAttributeType(attributeType);

            if (_propertyInfoForPropertyName.ContainsKey(implicitName))
            {
                return _propertyInfoForPropertyName[implicitName];
            }

            return null;
        }

        public PropertyInfo PropertyInfoForJsonApiAttributeName(string name)
        {
            // Look for a property set via the [JsonApiAttribute("name")] attribute
            if (_propertyInfoForJsonApiAttributeName.ContainsKey(name))
            {
                return _propertyInfoForJsonApiAttributeName[name];
            }

            var implicitName = ImplicitPropertyNameForJsonApiAttributeName(name);

            if (_propertyInfoForPropertyName.ContainsKey(implicitName))
            {
                return _propertyInfoForPropertyName[implicitName];
            }

            return null;
        }

        public PropertyInfo PropertyInfoForJsonApiRelationshipName(string name)
        {
            // Look for a property set via the [JsonApiAttribute("name")] attribute
            if (_propertyInfoForJsonApiRelationshipName.ContainsKey(name))
            {
                return _propertyInfoForJsonApiRelationshipName[name];
            }

            var implicitName = ImplicitPropertyNameForJsonApiRelationshipName(name);

            if (_propertyInfoForPropertyName.ContainsKey(implicitName))
            {
                return _propertyInfoForPropertyName[implicitName];
            }

            return null;
        }

        private static string ImplicitPropertyNameForJsonApiAttributeName(string name)
        {
            return name.Underscore().Pascalize();
        }

        private static string ImplicitPropertyNameForJsonApiRelationshipName(string name)
        {
            return name.Underscore().Pascalize();
        }

        private static string ImplicitPropertyNameForAttributeType(Type attributeType)
        {
            if (typeof(JsonApiIdAttribute) == attributeType)
            {
                return @"Id";
            }

            if (typeof(JsonApiTypeAttribute) == attributeType)
            {
                return @"Type";
            }

            throw new Exception(string.Format("No implicit property name found for [{0}]", attributeType.Name));
        }

        private static void InsertLookupTableEntry<T, T1>(IDictionary<T, T1> dictionary, T key, T1 value)
        {
            if (dictionary.ContainsKey(key))
            {
                throw new JsonApiUsageException(string.Format("The \"{0}\" attribute was specified more than once", key));
            }

            dictionary[key] = value;
        }

        // Reflect all properties of the _resourceType, building lookup tables for:
        // 1. Get the Property for a JsonApiAttribute named 'title'
        // 2. Get the Property with the [JsonApiId] attribute set
        // 3. Get the Property with name "Title"
        private void InitializeLookupTables()
        {
            var objectTypeProperties = ResourceType.GetProperties();

            foreach (var propertyInfo in objectTypeProperties)
            {
                var propertyAttrs = propertyInfo.GetCustomAttributes(typeof(JsonApiBaseAttribute), true);

                // this will be a list of 1 because AllowMultiple = false on the BaseAttribute class
                foreach (var propertyAttribute in propertyAttrs)
                {
                    if (propertyAttribute.GetType() == typeof(JsonApiAttributeAttribute))
                    {
                        // The "AttributeAttribute" stores the attribute name to map into the property
                        var jsonApiAttributeName = ((JsonApiAttributeAttribute)propertyAttribute).JsonApiAttributeName;
                        InsertLookupTableEntry(_propertyInfoForJsonApiAttributeName, jsonApiAttributeName, propertyInfo);
                    }
                    else if (propertyAttribute.GetType() == typeof(JsonApiRelationshipAttribute))
                    {
                        // The "RelationshipAttribute" stores the relationship name to map into the property
                        var jsonApiRelationshipName = ((JsonApiRelationshipAttribute)propertyAttribute).JsonApiRelationshipName;
                        InsertLookupTableEntry(_propertyInfoForJsonApiRelationshipName, jsonApiRelationshipName, propertyInfo);
                    }
                    else
                    {
                        InsertLookupTableEntry(_propertyInfoForAttributeType, propertyAttribute.GetType(), propertyInfo);
                    }
                }

                InsertLookupTableEntry(_propertyInfoForPropertyName, propertyInfo.Name, propertyInfo);
            }
        }
    }
}