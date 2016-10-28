using JsonApiNet.Tests.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonApiNet.Tests.Readme.CompoundDocument
{
    [TestClass]
    public class ReadmeCompoundDocumentWithoutIncludesTests
    {
        [TestMethod]
        public void ArticleFromJsonApiDocumentResourceTestWithoutIncludes()
        {
            var json = TestData.ReadmeCompoundDocumentWithoutIncludes();
            var document = JsonApi.Document<Article>(json, ignoreMissingRelationships: true);
            var article = document.Resource;
            Assert.AreEqual("JSON API paints my bikeshed!", article.Title);
        }
    }
}