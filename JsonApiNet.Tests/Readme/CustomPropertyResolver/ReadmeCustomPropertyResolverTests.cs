using System;
using System.Reflection;
using JsonApiNet.Resolvers;
using JsonApiNet.Tests.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonApiNet.Tests.Readme.CustomPropertyResolver
{
    [TestClass]
    public class ReadmeCustomPropertyResolverTests
    {
        [TestMethod]
        public void CustomPropertyResolverTest()
        {
            var json = TestData.ReadmeSingleResourceJson();
            var article = JsonApi.ResourceFromDocument<Article>(json, null, new NamingThingsIsHardResolver());
            Assert.AreEqual("JSON API paints my bikeshed!", article.ThingYouCallIt);
        }
    }

    public class Article
    {
        public string ThingYouCallIt { get; set; }
    }

    public class NamingThingsIsHardResolver : JsonApiPropertyResolver
    {
        public override PropertyInfo ResolveJsonApiAttribute(Type type, string attributeName)
        {
            if (type == typeof(Article) && attributeName == "title")
            {
                return type.GetProperty("ThingYouCallIt");
            }

            return base.ResolveJsonApiAttribute(type, attributeName);
        }
    }
}