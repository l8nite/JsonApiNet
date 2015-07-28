using System;
using System.Collections.Generic;

namespace JsonApiNet.Helpers
{
    public class PropertyLookupCacheManager
    {
        private Dictionary<Type, PropertyLookupCache> LookupCacheByType { get; set; }

        public PropertyLookupCache ForType(Type type)
        {
            if (LookupCacheByType == null)
            {
                LookupCacheByType = new Dictionary<Type, PropertyLookupCache>();
            }

            if (!LookupCacheByType.ContainsKey(type))
            {
                LookupCacheByType[type] = new PropertyLookupCache(type);
            }

            return LookupCacheByType[type];
        }
    }
}