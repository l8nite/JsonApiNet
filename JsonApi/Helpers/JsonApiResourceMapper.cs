using System;
using System.Collections;
using System.Reflection;
using JsonApi.Attributes;
using JsonApi.Components;
using JsonApi.Exceptions;

namespace JsonApi.Helpers
{
    internal class JsonApiResourceMapper
    {
        private readonly JsonApiDocument _document;
        private readonly ResourcePropertyResolver _resolver;

        public JsonApiResourceMapper(JsonApiDocument document, ResourcePropertyResolver resolver)
        {
            _document = document;
            _resolver = resolver;
        }

        public object MapJsonApiResource(JsonApiResource jsonApiResource)
        {
            var resource = (dynamic)Activator.CreateInstance(_resolver.ResourceType);

            var idProperty = _resolver.PropertyInfoForAttributeType(typeof(JsonApiIdAttribute));
            SetParseableStringProperty(resource, idProperty, jsonApiResource.Id);

            var typeProperty = _resolver.PropertyInfoForAttributeType(typeof(JsonApiTypeAttribute));
            SetProperty(resource, typeProperty, jsonApiResource.Type);

            MapAttributes(resource, jsonApiResource);
            MapRelationships(resource, jsonApiResource);

            return resource;
        }

        private static void SetParseableStringProperty(object resource, PropertyInfo property, object value)
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

            dynamic result = parseMethod.Invoke(null, new[] { value });
            SetProperty(resource, property, result);
        }

        private static void SetProperty(object resource, PropertyInfo property, object value)
        {
            if (property == null)
            {
                return;
            }

            property.SetMethod.Invoke(resource, new[] { value });
        }

        // be refactored to avoid repeating some of the class/list initialization work
        private void MapRelationships(object resource, JsonApiResource jsonApiResource)
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

        // TODO: this code is pretty similar to what lives in DocumentJsonConverter, can probably DRY it up
        // it's basically "instantiate a property resolver, determine if list or single element, map instances"
        private void MapRelationship(object resource, string relationshipName, JsonApiRelationship relationship)
        {
            // if it's an empty to-one relationship, we're done because there's nothing to map into it
            if (relationship.IsEmptyToOneRelationship)
            {
                return;
            }

            var relProperty = _resolver.PropertyInfoForJsonApiRelationshipName(relationshipName);

            // if the relationship is not mapped into a property on the resource then we're done
            if (relProperty == null)
            {
                return;
            }

            var relPropertyType = relProperty.PropertyType;
            var relResourceType = relPropertyType.GetSingleElementType();
            var relPropertyResolver = new ResourcePropertyResolver(relResourceType);
            var relResourceMapper = new JsonApiResourceMapper(_document, relPropertyResolver);

            if (relPropertyType.IsListType())
            {
                if (relationship.IsIncludedToOneRelationship)
                {
                    throw new JsonApiUsageException("Unable to map to-one relationship into a collection");
                }

                // instantiate relPropertyType, which is an IList<T1>
                var constructorInfo = relPropertyType.GetConstructor(Type.EmptyTypes);

                if (constructorInfo == null)
                {
                    throw new JsonApiUsageException(string.Format("No default constructor found for {0}", relPropertyType.Name));
                }

                var list = (IList)constructorInfo.Invoke(null);

                foreach (var resourceIdentifier in relationship.Data.ResourceIdentifiers)
                {
                    var includedResource = GetIncludedResourceByIdentifier(resourceIdentifier);
                    var mappedEntity = relResourceMapper.MapJsonApiResource(includedResource);
                    list.Add(mappedEntity);
                }

                SetProperty(resource, relProperty, list);
            }
            else
            {
                if (relationship.IsEmptyToManyRelationship || relationship.IsIncludedToManyRelationship)
                {
                    throw new JsonApiUsageException("Unable to map to-many relationship into a single object");
                }

                var resourceIdentifier = relationship.Data.ResourceIdentifiers[0]; // because IsSingleRelationship is true here
                var includedResource = GetIncludedResourceByIdentifier(resourceIdentifier);
                var mappedEntity = relResourceMapper.MapJsonApiResource(includedResource);

                SetProperty(resource, relProperty, mappedEntity);
            }
        }

        private JsonApiResource GetIncludedResourceByIdentifier(JsonApiResourceIdentifier resourceIdentifier)
        {
            var includedResource = _document.GetIncludedResourceByIdentifier(resourceIdentifier);

            if (includedResource == null)
            {
                throw new JsonApiFormatException(
                    string.Format(
                        "No included resource found for identifier: {0}",
                        resourceIdentifier));
            }

            return includedResource;
        }

        private void MapAttributes(object resource, JsonApiResource jsonApiResource)
        {
            if (jsonApiResource.Attributes == null)
            {
                return;
            }

            foreach (var kvp in jsonApiResource.Attributes)
            {
                var attrProperty = _resolver.PropertyInfoForJsonApiAttributeName(kvp.Key);
                SetProperty(resource, attrProperty, kvp.Value);
            }
        }
    }
}