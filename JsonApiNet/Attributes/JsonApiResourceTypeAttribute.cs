using System;

namespace JsonApiNet.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class JsonApiResourceTypeAttribute : Attribute
    {
        public JsonApiResourceTypeAttribute(string resourceTypeName)
        {
            JsonApiResourceTypeName = resourceTypeName;
        }

        public string JsonApiResourceTypeName { get; set; }
    }
}