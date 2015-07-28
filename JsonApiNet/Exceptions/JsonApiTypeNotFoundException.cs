using System;

namespace JsonApiNet.Exceptions
{
    public class JsonApiTypeNotFoundException : Exception
    {
        public JsonApiTypeNotFoundException(string message)
            : base(message)
        {
        }
    }
}