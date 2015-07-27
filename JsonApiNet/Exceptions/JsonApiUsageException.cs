using System;

namespace JsonApiNet.Exceptions
{
    public class JsonApiUsageException : Exception
    {
        public JsonApiUsageException(string message)
            : base(message)
        {
        }
    }
}