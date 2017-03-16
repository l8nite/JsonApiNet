using JsonApiNet.Tests.Data;
using JsonApiNet.TestSecondAssembly;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JsonApiNet.Tests.Readme.CompoundDocument;

namespace JsonApiNet.Tests.Readme.SecondAssembly
{
    [TestClass]
    public class SecondAssemblyTests
    {
        [TestMethod]
        public void SimpleTypeCanBeLoadedFromSecondAssembly()
        {
            var json = TestData.ValidGraphicNovelSimpleJson();
            var graphicNovel = JsonApi.ResourceFromDocument<GraphicNovel>(json);
            Assert.AreEqual("Tales from JSON API", graphicNovel.Title);
        }

        [TestMethod]
        public void SimpleTypeFromSecondAssemblyCanBeLoadedFromResource()
        {
            var json = TestData.ValidGraphicNovelSimpleJson();
            var graphicNovelResource = JsonApi.Document<GraphicNovel>(json);
            var graphicNovel = graphicNovelResource.Resource;
            Assert.AreEqual("Tales from JSON API", graphicNovel.Title);
        }

        [TestMethod]
        public void ComplexTypeCanBeLoadedFromSecondAssembly()
        {
            var json = TestData.ValidGraphicNovelCompoundJson();
            var graphicNovel = JsonApi.ResourceFromDocument<GraphicNovel>(json);

            Assert.AreEqual("Tales from JSON API", graphicNovel.Title);
            Assert.IsNotNull(graphicNovel.Illustrator);
            Assert.AreEqual("Stan", graphicNovel.Illustrator.FirstName);
            Assert.IsNotNull(graphicNovel.Illustrations);
            Assert.AreEqual(2, graphicNovel.Illustrations.Count);

            Assert.IsTrue(graphicNovel.Illustrations.Any(i => i.HorizontalResolution == 640));
            Assert.IsTrue(graphicNovel.Illustrations.Any(i => i.VerticalResolution == 720));
            Assert.IsTrue(graphicNovel.Illustrations.Any(i => i.ImageUri == "http://example.com/images/112.jpg"));
        }

        [TestMethod]
        public void ComplexTypeInSecondAssemblyCanBeLoadedFromResouce()
        {
            var json = TestData.ValidGraphicNovelCompoundJson();
            var graphicNovelDocument = JsonApi.Document<GraphicNovel>(json);
            var graphicNovel = graphicNovelDocument.Resource;

            Assert.AreEqual("Tales from JSON API", graphicNovel.Title);
            Assert.IsNotNull(graphicNovel.Illustrator);
            Assert.AreEqual("Stan", graphicNovel.Illustrator.FirstName);
            Assert.IsNotNull(graphicNovel.Illustrations);
            Assert.AreEqual(2, graphicNovel.Illustrations.Count);

            List<Illustration> illustrations = graphicNovel.Illustrations;

            Assert.IsTrue(illustrations.Any(i => i.HorizontalResolution == 640));
            Assert.IsTrue(illustrations.Any(i => i.VerticalResolution == 720));
            Assert.IsTrue(illustrations.Any(i => i.ImageUri == "http://example.com/images/112.jpg"));
        }

        [TestMethod]
        public void TypesCanBeLoadedFromMultipleAssemblies()
        {
            var json = TestData.ReadmeCompoundDocumentJson();
            var articles = JsonApi.ResourceFromDocument<List<Article>>(json);
            json = TestData.ValidGraphicNovelCompoundJson();
            var graphicNovel = JsonApi.ResourceFromDocument<GraphicNovel>(json);

            Assert.IsNotNull(articles);
            Assert.IsNotNull(graphicNovel);
        }

        [TestMethod]
        public void ObjectGraphCanSpanAssemblies()
        {
            var json = TestData.CrossAssemblyDocumentJson();

            // Because the Illustration type is defined in a separate assembly,
            // we must pass in a reference to the assembly that contains the
            // definition of Illustration.
            Assembly[] additionalAssemblies = {
                typeof(Illustration).GetTypeInfo().Assembly
            };

            var brochure = JsonApi.ResourceFromDocument<Brochure>(json, additionalAssemblies: additionalAssemblies);

            Assert.IsNotNull(brochure);
            Assert.IsNotNull(brochure.Illustrations);
            Assert.AreEqual(1, brochure.Illustrations.Count);

            var illustration = brochure.Illustrations.Single();
            Assert.AreEqual(640, illustration.HorizontalResolution);
        }

        [TestMethod]
        public void ResourceObjectGraphCanSpanAssemblies()
        {
            var json = TestData.CrossAssemblyDocumentJson();

            Assembly[] additionalAssemblies = {
                typeof(Illustration).GetTypeInfo().Assembly
            };

            var brochureDocument = JsonApi.Document<Brochure>(json, additionalAssemblies: additionalAssemblies);
            var brochure = brochureDocument.Resource;

            Assert.IsNotNull(brochure);
            Assert.IsNotNull(brochure.Illustrations);
            Assert.AreEqual(1, brochure.Illustrations.Count);

            var illustration = brochure.Illustrations[0];
            Assert.AreEqual(640, illustration.HorizontalResolution);
        }

        public class Brochure
        {
            public string Title { get; set; }

            public List<Illustration> Illustrations { get; set; }
        }
    }
}
