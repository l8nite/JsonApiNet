namespace JsonApi.Attributes
{
    public class JsonApiAttributeAttribute : JsonApiBaseAttribute
    {
        public JsonApiAttributeAttribute(string attributeName)
        {
            JsonApiAttributeName = attributeName;
        }

        public string JsonApiAttributeName { get; set; }
    }
}