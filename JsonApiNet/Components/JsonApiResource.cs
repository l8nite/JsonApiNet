using Newtonsoft.Json;

namespace JsonApiNet.Components
{
    public class JsonApiResource
    {
        private string _id;
        private string _type;

        public JsonApiResource()
        {
            ResourceIdentifier = new JsonApiResourceIdentifier(null, null);
        }

        [JsonProperty("id")]
        public string Id
        {
            get { return _id; }

            set
            {
                _id = value;
                ResourceIdentifier.Id = _id;
            }
        }

        [JsonProperty("type")]
        public string Type
        {
            get { return _type; }

            set
            {
                _type = value;
                ResourceIdentifier.Type = _type;
            }
        }

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