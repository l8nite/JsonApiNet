namespace JsonApiNet.Attributes
{
    public class JsonApiRelationshipAttribute : JsonApiPropertyAttribute
    {
        public JsonApiRelationshipAttribute(string relationshipName)
        {
            JsonApiRelationshipName = relationshipName;
        }

        public string JsonApiRelationshipName { get; set; }
    }
}