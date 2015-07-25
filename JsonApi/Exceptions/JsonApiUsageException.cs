using System;

namespace JsonApi.Exceptions
{
    public class JsonApiUsageException : Exception
    {
        public JsonApiUsageException(string message)
            : base(message)
        {
        }
    }
}