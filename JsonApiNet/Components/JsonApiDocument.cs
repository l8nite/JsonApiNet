using System.Collections.Generic;
using System.Linq;

namespace JsonApiNet.Components
{
    public class JsonApiDocument
    {
        public bool HasSingleResource { get; set; }

        public bool HasMultipleResources
        {
            get { return !HasSingleResource; }
        }

        public bool HasErrors
        {
            get { return Errors != null && Errors.Count > 0; }
        }

        public JsonApiErrors Errors { get; set; }

        public JsonApiMeta Meta { get; set; }

        public JsonApiJsonApi JsonApi { get; set; }

        public JsonApiLinks Links { get; set; }

        public List<JsonApiResource> Included { get; set; }

        // always stored as an array, but deserialized by the DocumentConverter and sets
        // the HasSingleResource or HasMultipleResources helpers to determine behavior
        public List<JsonApiResource> Data { get; set; }

        public dynamic Resource { get; set; }

        // note this glosses over some format issues, like if there are multiple included resources with the same identifier
        public JsonApiResource GetIncludedResourceByIdentifier(JsonApiResourceIdentifier id)
        {
            return Included?.FirstOrDefault(jsonApiResource => jsonApiResource.ResourceIdentifier.Equals(id));
        }
    }
}