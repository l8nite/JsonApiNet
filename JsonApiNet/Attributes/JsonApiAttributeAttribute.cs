namespace JsonApiNet.Attributes
{
    public class JsonApiAttributeAttribute : JsonApiPropertyAttribute
    {
        public JsonApiAttributeAttribute(string attributeName)
        {
            JsonApiAttributeName = attributeName;
        }

        public string JsonApiAttributeName { get; set; }
    }
}