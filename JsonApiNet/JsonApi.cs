using System;
using System.Reflection;
using JsonApiNet.Components;
using JsonApiNet.Helpers;
using JsonApiNet.Resolvers;

namespace JsonApiNet
{
    public static class JsonApi
    {
        public static JsonApiDocument Document(string json, 
                                                Type resultType, 
                                                JsonApiSettings settings = null, 
                                                Assembly[] additionalAssemblies = null)
        {
            var serializer = MakeSerializer(resultType, settings, additionalAssemblies ?? new Assembly[0]);

            // default is to skip resource creation
            if (serializer.Settings.CreateResource == null)
            {
                serializer.Settings.CreateResource = false;
            }

            return serializer.Document(json);
        }

        public static dynamic ResourceFromDocument(string json, 
                                                    Type resultType, 
                                                    JsonApiSettings settings = null, 
                                                    Assembly[] additionalAssemblies = null)
        {
            var serializer = MakeSerializer(resultType, settings, additionalAssemblies);
            serializer.Settings.CreateResource = true;

            return serializer.ResourceFromDocument(json);
        }

        public static JsonApiDocument Document<T>(
            string json,
            IJsonApiTypeResolver typeResolver = null,
            IJsonApiPropertyResolver propertyResolver = null,
            bool ignoreMissingRelationships = false,
            Assembly[] additionalAssemblies = null)
        {
            var settings = new JsonApiSettings
            {
                CreateResource = true,
                PropertyResolver = propertyResolver,
                TypeResolver = typeResolver,
                IgnoreMissingRelationships = ignoreMissingRelationships
            };

            var singleElementType = typeof(T).GetSingleElementType();

            return Document(json, singleElementType, settings, additionalAssemblies);
        }

        public static T ResourceFromDocument<T>(
            string json,
            IJsonApiTypeResolver typeResolver = null,
            IJsonApiPropertyResolver propertyResolver = null,
            bool ignoreMissingRelationships = false,
            Assembly[] additionalAssemblies = null)
        {
            var settings = new JsonApiSettings
            {
                CreateResource = true,
                PropertyResolver = propertyResolver,
                TypeResolver = typeResolver,
                IgnoreMissingRelationships = ignoreMissingRelationships
            };

            var singleElementType = typeof(T).GetSingleElementType();

            return ResourceFromDocument(json, singleElementType, settings, additionalAssemblies);
        }

        private static JsonApiNetSerializer MakeSerializer(
            Type resultType, 
            JsonApiSettings settings, 
            Assembly[] additionalAssemblies)
        {
            settings = settings ?? new JsonApiSettings();

            settings.ResultType = settings.ResultType ?? resultType;

            if (settings.TypeResolver == null)
            {
                settings.TypeResolver = new ReflectingTypeResolver(resultType, additionalAssemblies);
            }

            settings.PropertyResolver = settings.PropertyResolver ?? new JsonApiPropertyResolver();

            return new JsonApiNetSerializer(settings);
        }
    }
}