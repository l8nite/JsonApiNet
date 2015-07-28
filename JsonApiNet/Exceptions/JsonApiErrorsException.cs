using System;
using JsonApiNet.Components;

namespace JsonApiNet.Exceptions
{
    public class JsonApiErrorsException : Exception
    {
        public JsonApiErrorsException(JsonApiErrors errors)
            : base(errors.Message)
        {
            Errors = errors;
        }

        public JsonApiErrorsException(JsonApiErrors errors, Exception innerException)
            : base(errors.Message, innerException)
        {
            Errors = errors;
        }

        private JsonApiErrors Errors { get; set; }
    }
}