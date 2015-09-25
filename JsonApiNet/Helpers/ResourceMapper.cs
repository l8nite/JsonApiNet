using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JsonApiNet.Components;
using JsonApiNet.Exceptions;

namespace JsonApiNet.Helpers
{
    internal class ResourceMapper
    {
        private readonly JsonApiDocument _document;
        private readonly JsonApiSettings _settings;

        public ResourceMapper(JsonApiDocument document, JsonApiSettings settings)
        {
            _document = document;
            _settings = settings;
        }

        public dynamic ToObject(JsonApiResource jsonApiResource)
        {
            var typeResolver = _settings.TypeResolver;
            var resolvedType = typeResolver.ResolveType(jsonApiResource.Type);

            if (resolvedType == null)
            {
                throw new JsonApiTypeNotFoundException(string.Format("No type found for {0}", jsonApiResource.Type));
            }

            var resource = (dynamic)Activator.CreateInstance(resolvedType);

            var propResolver = _settings.PropertyResolver;

            var idProperty = propResolver.ResolveJsonApiId(resolvedType);
            SetParseableStringProperty(resource, idProperty, jsonApiResource.Id);

            var typeProperty = propResolver.ResolveJsonApiType(resolvedType);
            SetProperty(resource, typeProperty, jsonApiResource.Type);

            MapAttributes(resource, jsonApiResource);
            MapRelationships(resource, jsonApiResource);

            return resource;
        }

        private static void SetParseableStringProperty(dynamic resource, PropertyInfo property, object value)
        {
            if (property == null)
            {
                return;
            }

            var propertyType = property.PropertyType;

            if (propertyType == typeof(String))
            {
                SetProperty(resource, property, value);
                return;
            }

            // attempt to call "T.Parse(Id)" where T is the property's Type
            // assign the parsed result to the mapped property
            var parseMethod = propertyType.GetMethod(
                "Parse", 
                BindingFlags.Public | BindingFlags.Static, 
                null, 
                new[] { typeof(string) }, 
                null);

            if (parseMethod == null || parseMethod.ReturnType != propertyType)
            {
                return;
            }

            if (property.PropertyType == typeof(Guid) && value == null)
            {
                value = default(Guid).ToString();
            }

            dynamic result = parseMethod.Invoke(null, new[] { value });
            SetProperty(resource, property, result);
        }

        private static void SetProperty(dynamic resource, PropertyInfo property, object value)
        {
            if (property == null)
            {
                return;
            }

            property.GetSetMethod(true).Invoke(resource, new[] { value });
        }

        private void MapRelationships(dynamic resource, JsonApiResource jsonApiResource)
        {
            if (jsonApiResource.Relationships == null)
            {
                return;
            }

            foreach (var kvp in jsonApiResource.Relationships)
            {
                MapRelationship(resource, kvp.Key, kvp.Value);
            }
        }

        private void MapRelationship(dynamic resource, string relationshipName, JsonApiRelationship relationship)
        {
            // if it's an empty to-one relationship, we're done because there's nothing to map into it
            if (relationship.IsEmptyToOneRelationship)
            {
                return;
            }

            var propResolver = _settings.PropertyResolver;
            var relProperty = propResolver.ResolveJsonApiRelationship(((object)resource).GetType(), relationshipName);

            // if the relationship is not mapped into a property on the resource then we're done
            if (relProperty == null)
            {
                return;
            }

            if (relationship.IsIncludedToOneRelationship)
            {
                var resourceIdentifier = relationship.Data.ResourceIdentifiers[0]; // because IsSingleRelationship is true here
                var includedResource = GetIncludedResourceByIdentifier(resourceIdentifier);
                var mappedEntity = ToObject(includedResource);

                SetProperty(resource, relProperty, mappedEntity);
            }
            else
            {
                // we'll make a container of the mapped property type
                var elementType = relProperty.PropertyType.GetSingleElementType();
                var listType = typeof(List<>).MakeGenericType(new Type[] { elementType });
                var list = (IList)Activator.CreateInstance(listType);

                var jsonApiResources = relationship.Data.ResourceIdentifiers.Select(GetIncludedResourceByIdentifier);

                foreach (var jsonApiResource in jsonApiResources)
                {
                    list.Add(ToObject(jsonApiResource));
                }

                SetProperty(resource, relProperty, list);
            }
        }

        private JsonApiResource GetIncludedResourceByIdentifier(JsonApiResourceIdentifier resourceIdentifier)
        {
            var includedResource = _document.GetIncludedResourceByIdentifier(resourceIdentifier);

            if (includedResource == null)
            {
                throw new JsonApiFormatException(string.Format("No included resource found for identifier: {0}", resourceIdentifier));
            }

            return includedResource;
        }

        private void MapAttributes(dynamic resource, JsonApiResource jsonApiResource)
        {
            if (jsonApiResource.Attributes == null)
            {
                return;
            }

            foreach (var kvp in jsonApiResource.Attributes)
            {
                var attrProperty = _settings.PropertyResolver.ResolveJsonApiAttribute(((object)resource).GetType(), kvp.Key);
                SetProperty(resource, attrProperty, kvp.Value);
            }
        }
    }
}