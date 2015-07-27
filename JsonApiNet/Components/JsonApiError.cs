using Newtonsoft.Json;

namespace JsonApiNet.Components
{
    public class JsonApiError
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("links")]
        public JsonApiLinks Links { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("detail")]
        public string Detail { get; set; }

        [JsonProperty("source")]
        public JsonApiErrorSource Source { get; set; }

        [JsonProperty("meta")]
        public JsonApiMeta Meta { get; set; }

        public string Message
        {
            get
            {
                return string.Format("{0}: {1}", Title, Detail);
            }
        }
    }
}