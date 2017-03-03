using System;
using JsonApiNet.Resolvers;
using JsonApiNet.Tests.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonApiNet.Tests.Readme.ResourceTypeResolution
{
    [TestClass]
    public class ReadmeResourceTypeReolutionTests
    {
        [TestMethod]
        public void ContainerVsResourceTypeTest()
        {
            var json = TestData.ReadmeResourceTypeResolutionJson();

            var rainDrop = JsonApi.ResourceFromDocument<RainDrop>(json);
            Assert.IsTrue(((RainDrop)rainDrop).Splatter);
        }

        [TestMethod]
        public void CustomResourceTypeResolverTest()
        {
            var json = TestData.ReadmeResourceTypeResolutionJson();

            var lemonDrop = JsonApi.ResourceFromDocument<LemonDrop>(json, new BarneyTypeResolver());
            Assert.IsTrue(lemonDrop.Splatter);
        }
    }

    public class RainDrop
    {
        public bool Splatter { get; set; }
    }

    public class BarneyTypeResolver : IJsonApiTypeResolver
    {
        public Type ResolveType(string typeName)
        {
            return typeName == "rain_drops" ? typeof(LemonDrop) : null;
        }
    }

    public class LemonDrop
    {
        public bool Splatter { get; set; }
    }
}