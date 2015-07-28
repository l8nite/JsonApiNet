using System;
using JsonApiNet.Components;
using JsonApiNet.Exceptions;
using JsonApiNet.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonApiNet.JsonConverters
{
    internal class ResourceJsonConverter : JsonConverter
    {
        private readonly JsonApiSettings _settings;

        public ResourceJsonConverter(JsonApiSettings settings)
        {
            _settings = settings;
        }

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

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var resourceToken = JToken.Load(reader);

            if (resourceToken == null)
            {
                return null;
            }

            if (resourceToken.Type != JTokenType.Object)
            {
                throw new JsonApiFormatException("Individual resource element must be an Object");
            }

            var resourceObj = (JObject)resourceToken;

            var jsonApiResource = new JsonApiResource((string)resourceObj["type"], (string)resourceObj["id"]);

            jsonApiResource.Attributes = ReadAttributes(resourceObj["attributes"], jsonApiResource, serializer);
            jsonApiResource.Relationships = ReadProperty<JsonApiRelationships>(resourceObj, "relationships", serializer);
            jsonApiResource.Links = ReadProperty<JsonApiLinks>(resourceObj, "links", serializer);
            jsonApiResource.Meta = ReadProperty<JsonApiMeta>(resourceObj, "meta", serializer);

            return jsonApiResource;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(JsonApiResource);
        }

        private static T ReadProperty<T>(JToken token, string name, JsonSerializer serializer) where T : class
        {
            return (token == null || token[name] == null) ? null : token[name].ToObject<T>(serializer);
        }

        private JsonApiAttributes ReadAttributes(JToken attributesToken, JsonApiResource resource, JsonSerializer serializer)
        {
            if (attributesToken == null)
            {
                return null;
            }

            var resolvedType = _settings.TypeResolver.ResolveType(resource.Type);

            var attributesObject = (JObject)attributesToken;

            var obj = new JsonApiAttributes();

            foreach (var attributeProperty in attributesObject.Properties())
            {
                if (resolvedType != null)
                {
                    var propertyInfo = _settings.PropertyResolver.ResolveJsonApiAttribute(resolvedType, attributeProperty.Name);

                    if (propertyInfo != null)
                    {
                        obj[attributeProperty.Name] = attributeProperty.Value.ToObject(propertyInfo.PropertyType, serializer);
                    }
                    else
                    {
                        obj[attributeProperty.Name] = attributeProperty.Value.ToObject(typeof(object));
                    }
                }
                else
                {
                    obj[attributeProperty.Name] = attributeProperty.Value.ToObject(typeof(object));
                }
            }

            return obj;
        }
    }
}