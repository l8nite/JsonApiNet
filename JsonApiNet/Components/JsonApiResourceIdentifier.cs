using System;
using Newtonsoft.Json;

namespace JsonApiNet.Components
{
    public class JsonApiResourceIdentifier
    {
        public JsonApiResourceIdentifier(string type, string id)
        {
            Type = type;
            Id = id;
        }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("meta")]
        public JsonApiMeta Meta { get; set; }

        public override string ToString()
        {
            return string.Format("Type: {0}, Id: {1}", Type, Id);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var r = obj as JsonApiResourceIdentifier;
            if (r == null)
            {
                return false;
            }

            return Type.Equals(r.Type) && Id.Equals(r.Id);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(Type, Id).GetHashCode();
        }

        public bool Equals(JsonApiResourceIdentifier r)
        {
            if (r == null)
            {
                return false;
            }

            return Type.Equals(r.Type) && Id.Equals(r.Id);
        }
    }
}