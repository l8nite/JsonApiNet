using System;
using System.Collections.Generic;
using JsonApiNet.Components;
using JsonApiNet.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonApiNet.JsonConverters
{
    internal class ResourceLinkageJsonConverter : JsonConverter
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

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);

            if (token == null || !token.HasValues)
            {
                return null;
            }

            var linkage = new JsonApiResourceLinkage();

            switch (token.Type)
            {
                case JTokenType.Array:
                    linkage.IsSingleResourceIdentifier = false;
                    linkage.ResourceIdentifiers = token.ToObject<List<JsonApiResourceIdentifier>>();
                    break;
                case JTokenType.Object:
                    linkage.IsSingleResourceIdentifier = true;
                    linkage.ResourceIdentifiers = new List<JsonApiResourceIdentifier>
                        {
                            token.ToObject<JsonApiResourceIdentifier>()
                        };
                    break;
                default:
                    throw new JsonApiFormatException("Resource linkage was not null, an object, or an array");
            }

            return linkage;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(JsonApiResourceLinkage);
        }
    }
}