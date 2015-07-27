namespace JsonApiNet.Attributes
{
    public class JsonApiRelationshipAttribute : JsonApiBaseAttribute
    {
        public JsonApiRelationshipAttribute(string relationshipName)
        {
            JsonApiRelationshipName = relationshipName;
        }

        public string JsonApiRelationshipName { get; set; }
    }
}