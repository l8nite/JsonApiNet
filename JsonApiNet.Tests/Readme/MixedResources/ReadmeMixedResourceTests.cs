using System;
using System.Collections.Generic;
using JsonApiNet.Tests.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonApiNet.Tests.Readme.MixedResources
{
    [TestClass]
    public class ReadmeMixedResourcesTests
    {
        [TestMethod]
        public void MixedResourcesWithCommonInterfaceTest()
        {
            var json = TestData.ReadmeMixedResourcesJson();
            var titled = JsonApi.ResourceFromDocument<List<ITitled>>(json);

            Assert.IsNotNull(titled);
            Assert.AreEqual(3, titled.Count);

            Assert.AreEqual("JSON API paints my bikeshed!", titled[0].Title);
            Assert.AreEqual("and I wrote a book about it...", titled[1].Title);
            Assert.AreEqual("which was featured in a magazine!", titled[2].Title);
        }
    }

    public interface ITitled
    {
        string Title { get; set; }
    }

    public class Article : ITitled
    {
        public Guid Id { get; set; }

        public string Title { get; set; }
    }

    public class Book : ITitled
    {
        public string Title { get; set; }
    }

    public class Magazine : ITitled
    {
        public string Title { get; set; }
    }
}