using System;
using System.Collections;
using System.Collections.Generic;
using JsonApi.Components;
using JsonApi.Exceptions;
using JsonApi.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonApi.JsonConverters
{
    internal class DocumentJsonConverter : JsonConverter
    {
        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        // objectType is JsonApiDocument<T> where T is the desired Resource class
        public override object ReadJson(JsonReader reader, Type jsonApiDocumentType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            var document = (JsonApiDocument)Activator.CreateInstance(jsonApiDocumentType);

            MapTopLevelElements(document, token);

            var dataToken = token["data"];

            if (dataToken == null)
            {
                return document;
            }

            var resourceType = GetIndividualResourceTypeForJsonApiDocument(jsonApiDocumentType);
            var hasResourceType = resourceType != null;

            JsonSerializer cerealizer;
            ResourcePropertyResolver resolver = null;

            if (hasResourceType)
            {
                // sneaky, we're passing an attribute property resolver for the generic type T from JsonApiDocument<T>
                // into the contract resolver, which passes it to the AttributesJsonConverter, which can deserialize
                // JsonApiAttributes objects using the complex objects found in "T"'s [JsonApiAttribute] mappings.
                resolver = new ResourcePropertyResolver(resourceType);
                cerealizer = new JsonSerializer
                    {
                        ContractResolver = new JsonApiAttributesContractResolver(resolver)
                    };
            }
            else
            {
                cerealizer = serializer;
            }

            // the "data" member can be an array or a single resource, we'll store it as a List<JsonApiResource>
            // and set some boolean flags appropriately so that callers can do the right thing
            if (dataToken.Type == JTokenType.Array)
            {
                document.HasSingleResource = false;
                document.Data = dataToken.ToObject<List<JsonApiResource>>(cerealizer);
            }
            else
            {
                document.HasSingleResource = true;
                document.Data = new List<JsonApiResource>
                    {
                        dataToken.ToObject<JsonApiResource>(cerealizer)
                    };
            }

            // if they're trying to DeserializeObject<JsonApiDocument<T>>, we now have everything we need to build Resources of type T
            if (hasResourceType)
            {
                ((dynamic)document).Resource = MapDocumentToGenericType(document, jsonApiDocumentType, resolver);
            }

            return document;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(JsonApiDocument<>) || objectType == typeof(JsonApiDocument);
        }

        // At the point of this method being called, the PropertyResolver should be instantiated for
        // the specific individual resource type of the document
        public dynamic MapDocumentToGenericType(JsonApiDocument document, Type jsonApiDocumentType, ResourcePropertyResolver resolver)
        {
            var genericType = jsonApiDocumentType.GetSingleGenericType();
            var isListType = genericType.IsListType();

            if ((document.HasSingleResource && isListType) || (document.HasMultipleResources && !isListType))
            {
                throw new JsonApiUsageException(
                    "T from JsonApiDocument<T> is an IList; however, this document represents a single resource");
            }

            if (document.HasSingleResource)
            {
                return MapJsonApiResource(document, document.Data[0], resolver);
            }

            // instantiate genericType, which should be an IList<T1>
            var constructorInfo = genericType.GetConstructor(Type.EmptyTypes);

            if (constructorInfo == null)
            {
                throw new JsonApiUsageException(string.Format("No default constructor found for {0}", genericType.Name));
            }

            var list = (IList)constructorInfo.Invoke(null);

            // map each individual JsonApiResource from document.Data
            foreach (var resource in document.Data)
            {
                list.Add(MapJsonApiResource(document, resource, resolver));
            }

            return list;
        }

        private static void MapTopLevelElements(JsonApiDocument document, JToken token)
        {
            // use the standard Json.NET converters to handle all of these
            document.Errors = token["errors"] != null ? token["errors"].ToObject<JsonApiErrors>() : null;
            document.Meta = token["meta"] != null ? token["meta"].ToObject<JsonApiMeta>() : null;
            document.JsonApi = token["jsonapi"] != null ? token["jsonapi"].ToObject<JsonApiJsonApi>() : null;
            document.Links = token["links"] != null ? token["links"].ToObject<JsonApiLinks>() : null;
            document.Included = token["included"] != null ? token["included"].ToObject<List<JsonApiResource>>() : null;
        }

        // jsonApiDocumentType is "JsonApiDocument<T>"
        // if T is an ICollection<T1>, return T1, otherwise return T
        private static Type GetIndividualResourceTypeForJsonApiDocument(Type jsonApiDocumentType)
        {
            var typeOfT = jsonApiDocumentType.GetSingleGenericType();
            return typeOfT == null ? null : typeOfT.GetSingleElementType();
        }

        private static object MapJsonApiResource(
            JsonApiDocument document, 
            JsonApiResource jsonApiResource, 
            ResourcePropertyResolver resolver)
        {
            var x = new JsonApiResourceMapper(document, resolver);
            return x.MapJsonApiResource(jsonApiResource);
        }
    }
}