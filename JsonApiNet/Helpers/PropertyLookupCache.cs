using System;
using System.Collections.Generic;
using System.Reflection;
using JsonApiNet.Attributes;
using JsonApiNet.Exceptions;

namespace JsonApiNet.Helpers
{
    public class PropertyLookupCache
    {
        // look up properties with [JsonApiId] or [JsonApiType] attributes
        public Dictionary<Type, PropertyInfo> PropertyInfoForAttributeType;

        // look up properties by "name" that have [JsonApiAttribute("name")]
        public Dictionary<string, PropertyInfo> PropertyInfoForJsonApiAttributeName;

        // look up properties by "name" that have [JsonApiRelationship("name")]
        public Dictionary<string, PropertyInfo> PropertyInfoForJsonApiRelationshipName;

        // look up properties by name
        public Dictionary<string, PropertyInfo> PropertyInfoForPropertyName;

        public PropertyLookupCache(Type type)
        {
            PropertyInfoForPropertyName = new Dictionary<string, PropertyInfo>();
            PropertyInfoForJsonApiAttributeName = new Dictionary<string, PropertyInfo>();
            PropertyInfoForJsonApiRelationshipName = new Dictionary<string, PropertyInfo>();
            PropertyInfoForAttributeType = new Dictionary<Type, PropertyInfo>();

            InitializeLookupTables(type);
        }

        private static void InsertLookupTableEntry<T, T1>(IDictionary<T, T1> dictionary, T key, T1 value)
        {
            if (dictionary.ContainsKey(key))
            {
                throw new JsonApiUsageException(string.Format("The \"{0}\" attribute was specified more than once", key));
            }

            dictionary[key] = value;
        }

        private void InitializeLookupTables(Type type)
        {
            var objectTypeProperties = type.GetProperties();

            foreach (var propertyInfo in objectTypeProperties)
            {
                var propertyAttrs = propertyInfo.GetCustomAttributes(typeof(JsonApiPropertyAttribute), true);

                // this will be a list of 1 because AllowMultiple = false on the BaseAttribute class
                foreach (var propertyAttribute in propertyAttrs)
                {
                    if (propertyAttribute.GetType() == typeof(JsonApiAttributeAttribute))
                    {
                        // The "AttributeAttribute" stores the attribute name to map into the property
                        var jsonApiAttributeName = ((JsonApiAttributeAttribute)propertyAttribute).JsonApiAttributeName;
                        InsertLookupTableEntry(PropertyInfoForJsonApiAttributeName, jsonApiAttributeName, propertyInfo);
                    }
                    else if (propertyAttribute.GetType() == typeof(JsonApiRelationshipAttribute))
                    {
                        // The "RelationshipAttribute" stores the relationship name to map into the property
                        var jsonApiRelationshipName = ((JsonApiRelationshipAttribute)propertyAttribute).JsonApiRelationshipName;
                        InsertLookupTableEntry(PropertyInfoForJsonApiRelationshipName, jsonApiRelationshipName, propertyInfo);
                    }
                    else
                    {
                        InsertLookupTableEntry(PropertyInfoForAttributeType, propertyAttribute.GetType(), propertyInfo);
                    }
                }

                InsertLookupTableEntry(PropertyInfoForPropertyName, propertyInfo.Name, propertyInfo);
            }
        }
    }
}