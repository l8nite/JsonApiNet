using System;
using JsonApiNet.Resolvers;

namespace JsonApiNet.Helpers
{
    public class JsonApiSettings
    {
        public IJsonApiTypeResolver TypeResolver { get; set; }

        public IJsonApiPropertyResolver PropertyResolver { get; set; }

        public Type ResultType { get; set; }

        public bool? CreateResource { get; set; }

        public bool IgnoreMissingRelationships { get; set; }
    }
}