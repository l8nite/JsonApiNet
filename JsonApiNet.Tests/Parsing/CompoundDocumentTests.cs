using System.Collections.Generic;
using JsonApiNet.Components;
using JsonApiNet.Tests.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace JsonApiNet.Tests.Parsing
{
    [TestClass]
    public class CompoundDocumentTests
    {
        private JsonApiDocument _document;

        [TestInitialize]
        public void TestInitialize()
        {
            _document = JsonConvert.DeserializeObject<JsonApiDocument>(TestData.ValidDocumentCompoundJson());
        }

        [TestMethod]
        public void DeserializObjectReturnsAnObject()
        {
            Assert.IsNotNull(_document);
        }

        [TestMethod]
        public void DataIsList()
        {
            Assert.IsInstanceOfType(_document.Data, typeof(List<JsonApiResource>));
            Assert.AreEqual(1, _document.Data.Count);
        }

        [TestMethod]
        public void Meta()
        {
            Assert.AreEqual("api", _document.Meta["json"]);
        }

        [TestMethod]
        public void JsonApi()
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
            Assert.AreEqual("7", resource.Relationships["comments"].Links["related"].Meta["upvotes"]);
            Assert.AreEqual("comments", resource.Relationships["comments"].Data.ResourceIdentifiers[0].Type);
            Assert.AreEqual("5", resource.Relationships["comments"].Data.ResourceIdentifiers[0].Id);
            Assert.AreEqual("comments", resource.Relationships["comments"].Data.ResourceIdentifiers[1].Type);
            Assert.AreEqual("12", resource.Relationships["comments"].Data.ResourceIdentifiers[1].Id);
        }

        [TestMethod]
        public void Included()
        {
            var included = _document.Included;
            Assert.AreEqual(3, included.Count);

            var person = included[0];
            Assert.AreEqual("9", person.Id);
            Assert.AreEqual("people", person.Type);
            Assert.AreEqual("Dan", person.Attributes["first-name"]);
            Assert.AreEqual("Gebhardt", person.Attributes["last-name"]);
            Assert.AreEqual("dgeb", person.Attributes["twitter"]);
            Assert.AreEqual("http://example.com/people/9", person.Links["self"].Href);

            var comment1 = included[1];
            Assert.AreEqual("First!", comment1.Attributes["body"]);

            var comment2 = included[2];
            Assert.AreEqual("I like XML better", comment2.Attributes["body"]);
        }
    }
}