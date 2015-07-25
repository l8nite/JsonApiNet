using System;
using JsonApi.Components;
using JsonApi.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JsonApi.JsonConverters
{
    internal class JsonApiAttributesContractResolver : DefaultContractResolver
    {
        private readonly ResourcePropertyResolver _resolver;

        public JsonApiAttributesContractResolver(ResourcePropertyResolver resolver)
        {
            _resolver = resolver;
        }

        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (objectType == null || !typeof(JsonApiAttributes).IsAssignableFrom(objectType))
            {
                // use the standard converter for everything except JsonApiAttributes
                return base.ResolveContractConverter(objectType);
            }

            return new AttributesJsonConverter(_resolver);
        }
    }
}