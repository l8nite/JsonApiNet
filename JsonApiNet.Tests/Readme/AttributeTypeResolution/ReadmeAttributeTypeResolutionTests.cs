using System.Collections.Generic;
using JsonApiNet.Tests.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonApiNet.Tests.Readme.AttributeTypeResolution
{
    [TestClass]
    public class ReadmeAttributeTypeResolutionTests
    {
        [TestMethod]
        public void ArticleFromDocumentTest()
        {
            var json = TestData.ReadmeAttributeTypeResolutionJson();
            var ghostBuster = JsonApi.ResourceFromDocument<GhostBuster>(json);
            Assert.AreEqual("I collect spores, molds, and fungus.", ghostBuster.Quotes[0]);
        }
    }

    public class GhostBuster
    {
        public string Id { get; set; }

        public List<string> Quotes { get; set; }
    }
}