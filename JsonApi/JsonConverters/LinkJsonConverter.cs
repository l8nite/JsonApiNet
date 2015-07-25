using System;
using System.Collections.Generic;
using JsonApi.Components;
using JsonApi.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonApi.JsonConverters
{
    internal class LinkJsonConverter : JsonConverter
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
            var linksToken = JToken.Load(reader);

            if (linksToken.Type == JTokenType.String)
            {
                return new JsonApiLink
                    {
                        Href = (string)linksToken, 
                        Meta = null
                    };
            }

            if (linksToken.Type != JTokenType.Object)
            {
                throw new JsonApiFormatException("The 'link' member was not a simple string or object");
            }

            var jsonApiLink = new JsonApiLink
                {
                    Href = (string)linksToken["href"], 
                };

            if (linksToken["meta"] != null)
            {
                jsonApiLink.Meta = linksToken["meta"].ToObject<Dictionary<string, object>>();
            }

            return jsonApiLink;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(JsonApiLink);
        }
    }
}