using JsonApi.Components;
using JsonApi.Tests.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace JsonApi.Tests.Parsing
{
    [TestClass]
    public class SimpleDocumentTests
    {
        private JsonApiDocument _document;

        [TestInitialize]
        public void TestInitialize()
        {
            _document = JsonConvert.DeserializeObject<JsonApiDocument>(TestData.ValidDocumentSimpleJson());
            Assert.IsFalse(_document.HasErrors);
        }

        [TestMethod]
        public void DeserializObjectReturnsAnObject()
        {
            Assert.IsNotNull(_document);
        }

        [TestMethod]
        public void NonDataAttributesAreNull()
        {
            Assert.IsNull(_document.Errors);
            Assert.IsNull(_document.Meta);
            Assert.IsNull(_document.JsonApi);
            Assert.IsNull(_document.Links);
            Assert.IsNull(_document.Included);
        }

        [TestMethod]
        public void DataIsParsed()
        {
            var data = _document.Data[0];

            Assert.IsNotNull(data);
            Assert.AreEqual("42", data.Id);
            Assert.AreEqual("articles", data.Type);
            Assert.AreEqual("JSON API paints my bikeshed!", data.Attributes["title"]);
            Assert.AreEqual("http://example.com/articles/11", data.Links["self"].Href);
        }
    }
}