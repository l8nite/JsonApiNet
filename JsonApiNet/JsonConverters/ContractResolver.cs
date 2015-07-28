using System;
using JsonApiNet.Components;
using JsonApiNet.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JsonApiNet.JsonConverters
{
    internal class ContractResolver : DefaultContractResolver
    {
        private readonly JsonApiSettings _jsonApiSettings;

        public ContractResolver(JsonApiSettings jsonApiSettings)
        {
            _jsonApiSettings = jsonApiSettings;
        }

        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (objectType == typeof(JsonApiDocument))
            {
                return new DocumentJsonConverter(_jsonApiSettings);
            }

            if (objectType == typeof(JsonApiResource))
            {
                return new ResourceJsonConverter(_jsonApiSettings);
            }

            if (objectType == typeof(JsonApiLink))
            {
                return new LinkJsonConverter();
            }

            if (objectType == typeof(JsonApiResourceLinkage))
            {
                return new ResourceLinkageJsonConverter();
            }

            return base.ResolveContractConverter(objectType);
        }
    }
}