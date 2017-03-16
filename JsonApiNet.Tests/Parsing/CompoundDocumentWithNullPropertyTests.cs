using System.Collections.Generic;
using JsonApiNet.Components;
using JsonApiNet.Tests.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonApiNet.Tests.Parsing
{
    [TestClass]
    public class CompoundDocumentWithNullPropertyTests
    {
        private JsonApiDocument _document;

        [TestInitialize]
        public void TestInitialize()
        {
            var json = TestData.ValidDocumentCompoundJsonWithNullProperty();
            _document = JsonApi.Document(json, typeof(CompoundDocumentWithNullPropertyTests));
        }

        [TestMethod]
        public void DeserializObjectReturnsAnObjectTest()
        {
            Assert.IsNotNull(_document);
        }

        [TestMethod]
        public void DataIsListTest()
        {
            Assert.IsInstanceOfType(_document.Data, typeof(List<JsonApiResource>));
            Assert.AreEqual(1, _document.Data.Count);
        }

        [TestMethod]
        public void MetaTest()
        {
            Assert.AreEqual("api", _document.Meta["json"]);
        }

        [TestMethod]
        public void JsonApiTest()
        {
            Assert.AreEqual("1.0", _document.JsonApi["version"]);
        }

        [TestMethod]
        public void Data()
        {
            var resource = _document.Data[0];
            Assert.AreEqual("1", resource.Id);
            Assert.AreEqual("articles", resource.Type);
            Assert.AreEqual("JSON API paints my bikeshed!", resource.Attributes["title"]);
            Assert.AreEqual("http://example.com/articles/1", resource.Links["self"].Href);
        }

        [TestMethod]
        public void DataRelationships()
        {
            var resource = _document.Data[0];
            Assert.IsNotNull(resource.Relationships);

            Assert.IsNotNull(resource.Relationships["author"]);
            Assert.AreEqual("http://example.com/articles/1/relationships/author", resource.Relationships["author"].Links["self"].Href);
            Assert.AreEqual("http://example.com/articles/1/author", resource.Relationships["author"].Links["related"].Href);
            Assert.AreEqual("people", resource.Relationships["author"].Data.ResourceIdentifiers[0].Type);
            Assert.AreEqual("9", resource.Relationships["author"].Data.ResourceIdentifiers[0].Id);

            Assert.IsNotNull(resource.Relationships["comments"]);
            Assert.AreEqual("http://example.com/articles/1/relationships/comments", resource.Relationships["comments"].Links["self"].Href);
            Assert.AreEqual("http://example.com/articles/1/comments", resource.Relationships["comments"].Links["related"].Href);
            Assert.IsNull(resource.Relationships["comments"].Data);
        }

        [TestMethod]
        public void Included()
        {
            var included = _document.Included;
            Assert.AreEqual(1, included.Count);

            var person = included[0];
            Assert.AreEqual("9", person.Id);
            Assert.AreEqual("people", person.Type);
            Assert.AreEqual("Dan", person.Attributes["first-name"]);
            Assert.AreEqual("Gebhardt", person.Attributes["last-name"]);
            Assert.AreEqual("dgeb", person.Attributes["twitter"]);
            Assert.AreEqual("http://example.com/people/9", person.Links["self"].Href);
        }
    }
}