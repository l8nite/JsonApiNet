using System;
using JsonApiNet.Attributes;
using JsonApiNet.Tests.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonApiNet.Tests.Readme.IdTypePropertyResolution
{
    [TestClass]
    public class ReadmeIdTypePropertyResolutionTests
    {
        [TestMethod]
        public void MappedIdAndTypeTest()
        {
            var json = TestData.ReadmeSingleResourceJson();
            var article = JsonApi.ResourceFromDocument<Article>(json);
            Assert.AreEqual(Guid.Parse("30cd428f-1a3b-459b-a9a8-0ca87c14dd31"), article.Identifier);
            Assert.AreEqual("articles", article.ResourceType);
        }
    }

    public class Article {
        [JsonApiId]
        public Guid Identifier { get; set; }

        [JsonApiType]
        public string ResourceType { get; set; }
    }
}