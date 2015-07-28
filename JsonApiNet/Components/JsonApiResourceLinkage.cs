using System.Collections.Generic;

namespace JsonApiNet.Components
{
    /*
     * Resource linkage MUST be represented as one of the following:
     *  - null for empty to-one relationships.
     *  - an empty array ([]) for empty to-many relationships.
     *  - a single resource identifier object for non-empty to-one relationships.
     *  - an array of resource identifier objects for non-empty to-many relationships.
     *  
     * This class stores the linked ResourceIdentifiers as a List<JsonApiResourceIdentifier>
     * and exposes some boolean properties for determining whether it represented a single
     * element relationship or not.
     * 
     * The boolean properties are set by the ResourceLinkageJsonConverter
     */
    public class JsonApiResourceLinkage
    {
        public List<JsonApiResourceIdentifier> ResourceIdentifiers { get; set; }

        public bool IsSingleResourceIdentifier { get; set; }

        public bool IsMultipleResourceIdentifier
        {
            get { return !IsSingleResourceIdentifier; }
        }
    }
}