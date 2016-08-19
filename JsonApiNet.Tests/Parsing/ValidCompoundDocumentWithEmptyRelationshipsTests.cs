using System.Collections.Generic;
using JsonApiNet.Tests.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonApiNet.Tests.Readme.ValidCompoundDocumentWithEmptyRelationships
{
    [TestClass]
    public class ValidCompundDocumentWithEmptyRelationshipsTests
    {
        [TestMethod]
        public void CompoundDocumentNullToOneRelationshipTest()
        {
            var json = TestData.ValidCompoundDocumentWithEmptyRelationshipsJson();
            var articles = JsonApi.ResourceFromDocument<List<Article>>(json);

            Assert.IsNotNull(articles);
            Assert.AreEqual(1, articles.Count);

            var article = articles[0];

            Assert.IsNull(article.Author);
        }

        [TestMethod]
        public void CompoundDocumentEmptyToManyRelationshipTest()
        {
            var json = TestData.ValidCompoundDocumentWithEmptyRelationshipsJson();
            var articles = JsonApi.ResourceFromDocument<List<Article>>(json);

            Assert.IsNotNull(articles);
            Assert.AreEqual(1, articles.Count);

            var article = articles[0];

            Assert.AreEqual(0, article.Comments.Count);
        }
    }

    public class Article
    {
        public string Title { get; set; }

        public Person Author { get; set; }

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