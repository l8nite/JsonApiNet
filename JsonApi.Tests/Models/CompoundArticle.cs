using System.Collections.Generic;
using JsonApi.Attributes;

namespace JsonApi.Tests.Models
{
    public class CompoundArticle
    {
        public int Id { get; set; }

        public string Title { get; set; }

        [JsonApiRelationship("author")]
        public Person WrittenBy { get; set; }

        // implicitly-defined JsonApiRelationship
        public List<Comment> Comments { get; set; }
    }
}