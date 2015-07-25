using Newtonsoft.Json;

namespace JsonApi.Components
{
    public class JsonApiErrorSource
    {
        [JsonProperty("pointer")]
        public string Pointer { get; set; }

        [JsonProperty("parameter")]
        public string Parameter { get; set; }
    }
}