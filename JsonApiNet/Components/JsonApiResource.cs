using Newtonsoft.Json;

namespace JsonApiNet.Components
{
    public class JsonApiResource
    {
        public JsonApiResource(string type, string id)
        {
            Id = id;
            Type = type;
            ResourceIdentifier = new JsonApiResourceIdentifier(type, id);
        }

        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("type")]
        public string Type { get; private set; }

        [JsonProperty("attributes")]
        public JsonApiAttributes Attributes { get; set; }

        [JsonProperty("relationships")]
        public JsonApiRelationships Relationships { get; set; }

        [JsonProperty("links")]
        public JsonApiLinks Links { get; set; }

        [JsonProperty("meta")]
        public JsonApiMeta Meta { get; set; }

        public JsonApiResourceIdentifier ResourceIdentifier { get; private set; }
    }
}