using System.Collections.Generic;
using JsonApiNet.Components;
using JsonApiNet.Tests.Data;
using JsonApiNet.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace JsonApiNet.Tests.Conversion
{
    [TestClass]
    public class CompoundResourceTests
    {
        private JsonApiDocument<List<CompoundArticle>> _document;

        [TestInitialize]
        public void TestInitialize()
        {
            _document = JsonConvert.DeserializeObject<JsonApiDocument<List<CompoundArticle>>>(TestData.ValidDocumentCompoundJson());
        }

        [TestMethod]
        public void ConversionTest()
        {
            var articles = _document.Resource;

            Assert.AreEqual(1, articles.Count);

            var article = articles[0];
            Assert.AreEqual("JSON API paints my bikeshed!", article.Title);
            
            Assert.IsNotNull(article.WrittenBy);

            var author = article.WrittenBy;
            Assert.AreEqual("Dan", author.FirstName);
            Assert.AreEqual("Gebhardt", author.LastName);

            Assert.IsNotNull(article.Comments);

            var comments = article.Comments;
            Assert.AreEqual(2, comments.Count);
            Assert.AreEqual("First!", comments[0].Body);
            Assert.AreEqual("I like XML better", comments[1].Body);
        }
    }
}