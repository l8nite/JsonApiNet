using System;
using System.Collections.Generic;
using JsonApi.JsonConverters;
using Newtonsoft.Json;

namespace JsonApi.Components
{
    [JsonConverter(typeof(LinkJsonConverter))]
    public class JsonApiLink
    {
        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("meta")]
        public Dictionary<string, object> Meta { get; set; }

        public Uri Uri
        {
            get { return new Uri(Href); }
        }
    }
}