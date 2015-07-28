using System.Collections.Generic;
using JsonApiNet.Attributes;
using JsonApiNet.Tests.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonApiNet.Tests.Readme.RelationshipPropertyResolution
{
    [TestClass]
    public class ReadmeRelationshipPropertyResolutionTests
    {
        [TestMethod]
        public void RemappedRelationshipTest()
        {
            var json = TestData.ReadmeCompoundDocumentJson();
            var articles = JsonApi.ResourceFromDocument<List<Article>>(json);

            Assert.IsNotNull(articles);
            Assert.AreEqual(1, articles.Count);

            var article = articles[0];
            Assert.AreEqual("JSON API paints my bikeshed!", article.Title);

            var author = articles[0].WrittenBy;
            Assert.AreEqual("Gebhardt", author.LastName);
            Assert.AreEqual("Gebhardt", articles[0].WrittenBy.LastName); // this is the actual README line

            var comments = articles[0].Comments;
            Assert.AreEqual("I like XML better", comments[1].Body);
        }
    }

    public class Article
    {
        public string Title { get; set; }

        [JsonApiRelationship("author")]
        public Person WrittenBy { get; set; }

        public List<Comment> Comments { get; set; }
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class Comment
    {
        public string Body { get; set; }
    }
}