using System;
using System.Collections.Generic;
using JsonApiNet.Tests.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonApiNet.Tests.Readme.MultipleResources
{
    [TestClass]
    public class ReadmeMultipleResourcesTests
    {
        [TestMethod]
        public void ArticlesFromDocumentTest()
        {
            var json = TestData.ReadmeMultipleResourcesJson();
            var articles = JsonApi.ResourceFromDocument<List<Article>>(json);

            Assert.IsNotNull(articles);
            Assert.AreEqual(1, articles.Count);

            var article = articles[0];
            Assert.AreEqual("JSON API paints my bikeshed!", article.Title);
        }
    }

    public class Article
    {
        public Guid Id { get; set; }

        public string Title { get; set; }
    }
}