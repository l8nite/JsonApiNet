using System;

namespace JsonApiNet.Exceptions
{
    public class JsonApiFormatException : Exception
    {
        public JsonApiFormatException(string message)
            : base(message)
        {
        }

        public JsonApiFormatException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}