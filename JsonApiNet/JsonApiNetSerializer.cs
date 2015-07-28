using JsonApiNet.Components;
using JsonApiNet.Helpers;
using JsonApiNet.JsonConverters;
using Newtonsoft.Json;

namespace JsonApiNet
{
    public class JsonApiNetSerializer : IJsonApiNetSerializer
    {
        public JsonApiSettings Settings { get; set; }

        public JsonApiNetSerializer(JsonApiSettings settings)
        {
            Settings = settings;
        }

        public dynamic ResourceFromDocument(string json)
        {
            var document = Document(json);
            return document.Resource;
        }

        public JsonApiDocument Document(string json)
        {
            return JsonConvert.DeserializeObject<JsonApiDocument>(
                json, 
                new JsonSerializerSettings
                    {
                        ContractResolver = new ContractResolver(Settings)
                    });
        }
    }
}