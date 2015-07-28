using System;
using System.Reflection;
using Humanizer;
using JsonApiNet.Attributes;
using JsonApiNet.Helpers;

namespace JsonApiNet.Resolvers
{
    public class JsonApiPropertyResolver : IJsonApiPropertyResolver
    {
        public PropertyLookupCacheManager PropertyLookupCacheManager { get; private set; }

        public JsonApiPropertyResolver()
        {
            PropertyLookupCacheManager = new PropertyLookupCacheManager();
        }

        public virtual PropertyInfo ResolveJsonApiAttribute(Type type, string attributeName)
        {
            var propertyLookupCache = PropertyLookupCacheManager.ForType(type);
            var cache = propertyLookupCache.PropertyInfoForJsonApiAttributeName;

            // Look for a property set via the [JsonApiAttribute("name")] attribute
            if (cache.ContainsKey(attributeName))
            {
                return cache[attributeName];
            }

            // Look for a property name with the normalized form of the attributeName
            return PropertyInfoByName(propertyLookupCache, attributeName.Underscore().Pascalize());
        }

        public virtual PropertyInfo ResolveJsonApiRelationship(Type type, string relationshipName)
        {
            var propertyLookupCache = PropertyLookupCacheManager.ForType(type);
            var cache = propertyLookupCache.PropertyInfoForJsonApiRelationshipName;

            // Look for a property set via the [JsonApiRelationship("name")] attribute
            if (cache.ContainsKey(relationshipName))
            {
                return cache[relationshipName];
            }

            // Look for a property name with the normalized form of the relationshipName
            return PropertyInfoByName(propertyLookupCache, relationshipName.Underscore().Pascalize());
        }

        public virtual PropertyInfo ResolveJsonApiType(Type type)
        {
            var propertyLookupCache = PropertyLookupCacheManager.ForType(type);
            var cache = propertyLookupCache.PropertyInfoForAttributeType;

            var jsonApiTypeAttributeType = typeof(JsonApiTypeAttribute);

            if (cache.ContainsKey(jsonApiTypeAttributeType))
            {
                return cache[jsonApiTypeAttributeType];
            }

            return PropertyInfoByName(propertyLookupCache, @"Type");
        }

        public virtual PropertyInfo ResolveJsonApiId(Type type)
        {
            var propertyLookupCache = PropertyLookupCacheManager.ForType(type);
            var cache = propertyLookupCache.PropertyInfoForAttributeType;

            var jsonApiIdAttribute = typeof(JsonApiIdAttribute);

            if (cache.ContainsKey(jsonApiIdAttribute))
            {
                return cache[jsonApiIdAttribute];
            }

            return PropertyInfoByName(propertyLookupCache, @"Id");
        }

        protected static PropertyInfo PropertyInfoByName(PropertyLookupCache propertyLookupCache, string propertyName)
        {
            if (propertyLookupCache.PropertyInfoForPropertyName.ContainsKey(propertyName))
            {
                return propertyLookupCache.PropertyInfoForPropertyName[propertyName];
            }

            return null;
        }
    }
}