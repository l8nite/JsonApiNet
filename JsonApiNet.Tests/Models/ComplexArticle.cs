using System;
using JsonApiNet.Attributes;
using Newtonsoft.Json;

namespace JsonApiNet.Tests.Models
{
    public class ComplexArticle
    {
        [JsonApiId]
        public Guid Identifier { get; set; }

        [JsonApiType]
        public string ResourceType { get; set; }

        [JsonApiAttribute("title")]
        public string ArticleTitle { get; set; }

        // implicitly-defined JsonApiAttribute
        public Tidbits Tidbits { get; set; }
    }

    public class Tidbits
    {
        [JsonProperty("isbn")]
        public long IsbnNumber { get; set; }

        public string Genre { get; set; }
    }
}