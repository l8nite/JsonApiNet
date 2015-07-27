using System;

namespace JsonApiNet.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class JsonApiBaseAttribute : Attribute
    {
    }
}