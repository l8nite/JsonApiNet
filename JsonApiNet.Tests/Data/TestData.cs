using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonApiNet.Tests.Data
{
    public static class TestData
    {
        public static string ValidDocumentErrorsJson()
        {
            return ReadExample("ValidDocumentErrors.json");
        }

        public static string ValidDocumentSimpleJson()
        {
            return ReadExample("ValidDocumentSimple.json");
        }

        public static string ValidDocumentCompoundJson()
        {
            return ReadExample("ValidDocumentCompound.json");
        }

        private static string ReadExample(string key)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var resourceName = string.Format("JsonApi.Tests.Data.{0}", key);

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

        public static string ValidDocumentComplexTypesJson()
        {
            return ReadExample("ValidDocumentComplexTypes.json");
        }
    }
}