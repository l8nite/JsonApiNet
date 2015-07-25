using System;
using JsonApi.Components;
using JsonApi.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonApi.JsonConverters
{
    internal class AttributesJsonConverter : JsonConverter
    {
        private readonly ResourcePropertyResolver _resolver;

        public AttributesJsonConverter(ResourcePropertyResolver resolver)
        {
            _resolver = resolver;
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

        // This allows us to deserialize values in the 'attributes' for a JsonApiResource into complex types
        // using the standard Json.NET deserialization properties/etc.
        // This Converter is only used when it is registered, which only happens as part of the DocumentJsonConverter
        // when deserializing a JSON API document into a JsonApiDocument<T>. It reflects on T's properties to find
        // the appropriate object types to convert the JSON into.
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var attributesToken = JToken.Load(reader);

            if (attributesToken == null)
            {
                return null;
            }

            var attributesObject = (JObject)attributesToken;

            var obj = new JsonApiAttributes();

            foreach (var attributeProperty in attributesObject.Properties())
            {
                var propertyInfo = _resolver.PropertyInfoForJsonApiAttributeName(attributeProperty.Name);

                if (propertyInfo != null)
                {
                    obj[attributeProperty.Name] = attributeProperty.Value.ToObject(propertyInfo.PropertyType);
                }
            }

            return obj;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(JsonApiAttributes);
        }
    }
}