using Newtonsoft.Json;

namespace JsonApiNet.Components
{
    public class JsonApiRelationship
    {
        [JsonProperty("links")]
        public JsonApiLinks Links { get; set; }

        [JsonProperty("data")]
        public JsonApiResourceLinkage Data { get; set; }

        [JsonProperty("meta")]
        public JsonApiMeta Meta { get; set; }

        public bool IsEmptyToOneRelationship
        {
            get { return Data == null; }
        }

        public bool IsEmptyToManyRelationship
        {
            get { return Data.IsMultipleResourceIdentifier && Data.ResourceIdentifiers.Count == 0; }
        }

        public bool IsIncludedToOneRelationship
        {
            get { return Data.IsSingleResourceIdentifier; }
        }

        public bool IsIncludedToManyRelationship
        {
            get { return Data.IsMultipleResourceIdentifier; }
        }
    }
}