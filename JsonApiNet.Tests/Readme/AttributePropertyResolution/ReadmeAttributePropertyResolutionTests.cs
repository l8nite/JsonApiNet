using JsonApiNet.Attributes;
using JsonApiNet.Tests.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonApiNet.Tests.Readme.AttributePropertyResolution
{
    [TestClass]
    public class ReadmeAttributePropertyResolutionTests
    {
        [TestMethod]
        public void MappedTitleToSubjectTest()
        {
            var json = TestData.ReadmeSingleResourceJson();
            var article = JsonApi.ResourceFromDocument<Article>(json);
            Assert.AreEqual("JSON API paints my bikeshed!", article.Subject);
        }
    }

    public class Article
    {
        [JsonApiAttribute("title")]
        public string Subject { get; set; }
    }
}