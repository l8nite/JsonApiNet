using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonApiNet.Tests.Data
{
    public static class TestData
    {
        public static string ValidDocumentErrorsJson()
        {
            return ReadEmbeddedResource("ValidDocumentErrors.json");
        }

        public static string ValidDocumentSimpleJson()
        {
            return ReadEmbeddedResource("ValidDocumentSimple.json");
        }

        public static string ValidDocumentCompoundJson()
        {
            return ReadEmbeddedResource("ValidDocumentCompound.json");
        }

        public static string ValidDocumentComplexTypesJson()
        {
            return ReadEmbeddedResource("ValidDocumentComplexTypes.json");
        }

        public static string ValidCompoundDocumentWithEmptyRelationshipsJson()
        {
            return ReadEmbeddedResource("ValidCompoundDocumentWithEmptyRelationships.json");
        }

        public static string ReadmeSingleResourceJson()
        {
            return ReadEmbeddedResource("ReadmeSingleResource.json");
        }

        public static string ReadmeMultipleResourcesJson()
        {
            return ReadEmbeddedResource("ReadmeMultipleResources.json");
        }

        public static string ReadmeCompoundDocumentJson()
        {
            return ReadEmbeddedResource("ReadmeCompoundDocument.json");
        }

        public static string ReadmeMixedResourcesJson()
        {
            return ReadEmbeddedResource("ReadmeMixedResources.json");
        }

        public static string ReadmeAttributeTypeResolutionJson()
        {
            return ReadEmbeddedResource("ReadmeAttributeTypeResolution.json");
        }

        public static string ReadmeResourceTypeResolutionJson()
        {
            return ReadEmbeddedResource("ReadmeResourceTypeResolution.json");
        }

        private static string ReadEmbeddedResource(string key)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var resourceName = string.Format("JsonApiNet.Tests.Data.{0}", key);

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new InternalTestFailureException(string.Format("No resource was found with key '{0}'", key));
                }

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}