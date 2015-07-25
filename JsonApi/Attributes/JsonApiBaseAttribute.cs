using System;

namespace JsonApi.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class JsonApiBaseAttribute : Attribute
    {
    }
}