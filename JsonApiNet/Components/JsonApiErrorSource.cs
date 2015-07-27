using Newtonsoft.Json;

namespace JsonApiNet.Components
{
    public class JsonApiErrorSource
    {
        [JsonProperty("pointer")]
        public string Pointer { get; set; }

        [JsonProperty("parameter")]
        public string Parameter { get; set; }
    }
}