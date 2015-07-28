using JsonApiNet.Components;
using JsonApiNet.Exceptions;
using JsonApiNet.Tests.Data;
using JsonApiNet.Tests.Readme.AttributePropertyResolution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace JsonApiNet.Tests.Parsing
{
    [TestClass]
    public class ErrorsDocumentTests
    {
        private JsonApiDocument _document;

        [TestInitialize]
        public void TestInitialize()
        {
            var json = TestData.ValidDocumentErrorsJson();
            _document = JsonApi.Document(json);
        }

        [TestMethod]
        public void DeserializObjectReturnsAnObject()
        {
            Assert.IsNotNull(_document);
        }

        [TestMethod]
        public void ResourceFromDocumentThrows()
        {
            var json = TestData.ValidDocumentErrorsJson();
            try
            {
                var res = JsonApi.ResourceFromDocument<Article>(json);
                Assert.Fail("Did not throw JsonApiErrorsException!");
            }
            catch (JsonApiErrorsException e)
            {
                Assert.AreEqual("Error Title: Error details go here.", e.Message);
            }
        }

        [TestMethod]
        public void Errors()
        {
            Assert.IsTrue(_document.HasErrors);

            Assert.AreEqual("Error Title: Error details go here.", _document.Errors.Message);

            var errors = _document.Errors;
            Assert.AreEqual(1, errors.Count);

            var error = errors[0];
            Assert.AreEqual("42", error.Id);
            Assert.AreEqual("500", error.Status);
            Assert.AreEqual("ISE", error.Code);
            Assert.AreEqual("Error Title", error.Title);
            Assert.AreEqual("Error details go here.", error.Detail);
            Assert.AreEqual("http://go.to/error", error.Links["about"].Href);
            Assert.AreEqual("foo", error.Meta["trace"]);
        }
    }
}