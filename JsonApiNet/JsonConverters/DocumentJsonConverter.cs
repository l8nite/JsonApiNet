using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using JsonApiNet.Components;
using JsonApiNet.Exceptions;
using JsonApiNet.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonApiNet.JsonConverters
{
    internal class DocumentJsonConverter : JsonConverter
    {
        private readonly JsonApiSettings _settings;

        public DocumentJsonConverter(JsonApiSettings settings)
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
            var token = JToken.Load(reader);

            var document = new JsonApiDocument
                {
                    Errors = ReadProperty<JsonApiErrors>(token, "errors", serializer), 
                    Meta = ReadProperty<JsonApiMeta>(token, "meta", serializer), 
                    JsonApi = ReadProperty<JsonApiJsonApi>(token, "jsonapi", serializer), 
                    Links = ReadProperty<JsonApiLinks>(token, "links", serializer), 
                    Included = ReadProperty<List<JsonApiResource>>(token, "included", serializer)
                };

            if (document.HasErrors && _settings.CreateResource == true)
            {
                throw new JsonApiErrorsException(document.Errors);
            }

            var dataToken = token["data"];

            if (dataToken == null)
            {
                return document;
            }

            switch (dataToken.Type)
            {
                    // this is a multiple-resource document
                case JTokenType.Array:
                    document.HasSingleResource = false;
                    document.Data = dataToken.ToObject<List<JsonApiResource>>(serializer);
                    break;

                    // this is a single-resource document
                case JTokenType.Object:
                    document.HasSingleResource = true;
                    document.Data = new List<JsonApiResource>
                        {
                            dataToken.ToObject<JsonApiResource>(serializer)
                        };
                    break;
                default:
                    throw new JsonApiFormatException("The 'data' member was not an array or object");
            }

            if (_settings.CreateResource == true)
            {
                document.Resource = CreateResource(document);
            }

            return document;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(JsonApiDocument) || objectType == typeof(JsonApiDocument);
        }

        private static T ReadProperty<T>(JToken token, string name, JsonSerializer serializer) where T : class
        {
            return (token == null || token[name] == null) ? null : token[name].ToObject<T>(serializer);
        }

        private dynamic CreateResource(JsonApiDocument document)
        {
            if (document.HasSingleResource)
            {
                return CreateResource(document, document.Data[0]);
            }

            // else if (document.HasMultipleResources)
            var list = (IList)new List<dynamic>();

            if (_settings.ResultType != null)
            {
                // if ResourceFromDocument<List<T>> was used, ResultType will be T
                var listType = typeof(List<>).MakeGenericType(new[] { _settings.ResultType });
                list = (IList)Activator.CreateInstance(listType);
            }

            foreach (var jsonApiResource in document.Data)
            {
                list.Add(CreateResource(document, jsonApiResource));
            }

            return list;
        }

        private dynamic CreateResource(JsonApiDocument document, JsonApiResource jsonApiResource)
        {
            var mapper = new ResourceMapper(document, _settings);
            return mapper.ToObject(jsonApiResource);
        }
    }
}