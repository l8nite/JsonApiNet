using System;
using JsonApiNet.Components;
using JsonApiNet.Tests.Data;
using JsonApiNet.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace JsonApiNet.Tests.Conversion
{
    [TestClass]
    public class ComplexArticleTests
    {
        private JsonApiDocument<ComplexArticle> _document;

        [TestInitialize]
        public void TestInitialize()
        {
            _document = JsonConvert.DeserializeObject<JsonApiDocument<ComplexArticle>>(TestData.ValidDocumentComplexTypesJson());
        }

        [TestMethod]
        public void ConversionTest()
        {
            var article = _document.Resource;
            Assert.AreEqual(Guid.Parse("93efd274-72b7-4738-8446-7263a5d54d05"), article.Identifier);
            Assert.AreEqual("articles", article.ResourceType);
            Assert.AreEqual("JSON API paints my bikeshed!", article.ArticleTitle);
            Assert.AreEqual(512, article.Tidbits.IsbnNumber);
            Assert.AreEqual("News", article.Tidbits.Genre);
        }
    }
}
