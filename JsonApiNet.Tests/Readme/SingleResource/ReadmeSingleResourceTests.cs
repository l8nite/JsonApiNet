using System;
using JsonApiNet.Tests.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonApiNet.Tests.Readme.SingleResource
{
    [TestClass]
    public class ReadmeSingleResourceTests
    {
        [TestMethod]
        public void ArticleFromDocumentTest()
        {
            var json = TestData.ReadmeSingleResourceJson();
            var article = JsonApi.ResourceFromDocument<Article>(json);
            Assert.AreEqual("JSON API paints my bikeshed!", article.Title);
        }

        [TestMethod]
        public void ArticleFromJsonApiDocumentResourceTest()
        {
            var json = TestData.ReadmeSingleResourceJson();
            var document = JsonApi.Document<Article>(json);
            var article = document.Resource;
            Assert.AreEqual("JSON API paints my bikeshed!", article.Title);
        }
    }

    public class Article
    {
        public Guid Id { get; set; }

        public string Title { get; set; }
    }
}