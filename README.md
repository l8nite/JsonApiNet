JsonApiNet
---

---
### This is an in-progress branch for `v2.0` of the `JsonApiNet` library, which is under active development and subject to change.  See the `v1.0` branch for the initial library implementation and README. ###

### Major changes in `v2.0` will include support for custom Type and Property resolvers (and a more consistent implementation), a less annoying interface than `JsonConvert.DeserializeObject<JsonApiDocument<List<Article>>>`), support for heterogenous resource collections and support for dependency injection.

---

An easy-to-use, extensible C# library for JSON API documents. 

- Deserialization of complex attributes
- Compound document support for included resources 
- Automatic `Type` and `Property` resolution
- Attribute and relationship property re-mapping
- Full object graph for the JSON API document
- And more!

Written against **v1.0** of [the JSON API specification](http://jsonapi.org/format/)

Built on top of [Json.NET](https://github.com/JamesNK/Newtonsoft.Json) and also uses [Humanizer](https://github.com/MehdiK/Humanizer) for type and property inference.

> Note: This library doesn't provide a serializer (yet). There are a couple alternative serializers for JSON API out there, such as the [JsonApiMediaTypeFormatter](https://github.com/rmichela/JsonApiMediaTypeFormatter). I haven't done a ton of exploration in this area yet, but I would eventually like to extend this library to support both serialization and deserialization. If you're the author of one of these libraries and think it'd be a good fit to merge with this project, let me know.


Single Resource
---

Here is a trivial, single-resource, JSON API document:

    {
      "data": {
        "type": "articles",
        "id": "30cd428f-1a3b-459b-a9a8-0ca87c14dd31",
        "attributes": {
          "title": "JSON API paints my bikeshed!"
        }
      }
    }

And we want to parse this response and into an `Article` class:

    public class Article {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }

We do so by calling `ResourceFromDocument<T>`:

    var article = JsonApiNet.ResourceFromDocument<Article>(json);
    Assert.AreEqual("JSON API paints my bikeshed!", article.Title);


Multiple Resources
---
From the example above, changing `"data"` into an array gives us a collection with 1 resource in it:

    {
      "data": [{
        "type": "articles",
        "id": "30cd428f-1a3b-459b-a9a8-0ca87c14dd31",
        "attributes": {
          "title": "JSON API paints my bikeshed!"
        }
      }]
    }

This time, we want to parse this response into a `List<Article>`:  

    var articles = JsonApiNet.ResourceFromDocument<List<Article>>(json);
    Assert.AreEqual("JSON API paints my bikeshed!", articles[0].Title);

If your document's collection contains heterogenous resource types, you must ensure that the container given to `ResourceFromDocument<T>` can store them or else you will get exceptions at run-time.


Compound Documents
---
Here is a compound document with a collection of resources that have relationships to included data, meta data, links, and more. This is a list of articles. Each article has a relationship to an author and a list of comments.

    {
      "data": [{
        "type": "articles",
        "id": "30cd428f-1a3b-459b-a9a8-0ca87c14dd31",
        "attributes": {
          "title": "JSON API paints my bikeshed!"
        },
        "links": {
          "self": "http://example.com/articles/1"
        },
        "relationships": {
          "author": {
            "links": {
              "self": "http://example.com/articles/1/relationships/author",
              "related": "http://example.com/articles/1/author"
            },
            "data": { "type": "people", "id": "9" }
          },
          "comments": {
            "links": {
              "self": "http://example.com/articles/1/relationships/comments",
              "related": "http://example.com/articles/1/comments"
            },
            "data": [
              { "type": "comments", "id": "5" },
              { "type": "comments", "id": "12" }
            ]
          }
        }
      }],
      "included": [{
        "type": "people",
        "id": "9",
        "attributes": {
          "first-name": "Dan",
          "last-name": "Gebhardt",
          "twitter": "dgeb"
        },
        "links": {
          "self": "http://example.com/people/9"
        }
      }, {
        "type": "comments",
        "id": "5",
        "attributes": {
          "body": "First!"
        },
        "links": {
          "self": "http://example.com/comments/5"
        }
      }, {
        "type": "comments",
        "id": "12",
        "attributes": {
          "body": "I like XML better"
        },
        "links": {
          "self": "http://example.com/comments/12"
        }
      }]
    }

We already have an `Article` class from before, but let's extend it with some additional properties:

    public class Article {
        public string Title { get; set; }
        public Person Author { get; set; }
        public List<Comment> Comments { get; set; }
    }

A `Person` class for the author:

    public class Person {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

And a `Comment` class for the comments:

    public class Comment {
        public string Body { get; set; }
    }

Nothing changes in how we retrieve the `List<Article>`:

    var articles = JsonApiNet.ResourceFromDocument<List<Article>>(json);
    Assert.AreEqual("JSON API paints my bikeshed!", articles[0].Title);

The author and comments are available too!

    var author = articles[0].Author;
    Assert.AreEqual("Gebhardt", author.LastName);

    var comments = articles[0].Comments;
    Assert.AreEqual("I like XML better", comments[1].Body);


Property Resolution for Attributes
---
In the examples, you might have noticed that there is a case mismatch between the `"title"` attribute named in the document and the `Title` property on the `Article` class.

The `JsonApiPropertyResolver` is what's responsible for finding the correct property on your `Type`.

> The default resolution process is: 
> 
> 1. First look in the `Type` for properties with the `[JsonApiAttribute("name")]` attribute, where `"name"` matches the the key from the `"attributes"` object in the document. If a property is found, use that.
> 
> 2. Next, look in the `Type` for a name that matches a normalized version of the key from the `"attributes"` object in the document. The normalization applies `Underscore()` and `PascalCase()` to the key, e.g. `"first-name"` becomes `FirstName`. 

For example, we could map the `"title"` attribute into a property named `Subject`:

    public class Article {
        [JsonApiAttribute("title")]
        public string Subject { get; set; }
    }

If for some reason you can't annotate your classes with the `[JsonApiAttribute("name")]` attribute, then you will need to write a custom `IJsonApiPropertyResolver` and hand it off to the serializer when converting your document. For example:

    public class MonopolyResolver : IJsonApiPropertyResolver {
        public PropertyInfo ResolveAttribute(Type type, string attributeName) {
            if (attributeName == "monopoly") {
                return type.GetProperty("boardwalk");
            }
            
            return null;
        }
    }

You can use it like this:

    var boardGame = JsonApiNet.ResourceFromDocument<BoardGame>(
        json, 
        new SerializerSettings {
            PropertyResolver = new MonopolyResolver()
        }
    );

`"attributes"` object keys which do not resolve to a valid property are skipped.


Type Resolution for Attributes
---
When parsing the values of an `"attributes"` object, `JsonApiNet` will attempt to deserialize each value into the `Type` associated with the property that the `JsonApiPropertyResolver` chose.

For example, if we have the following document:

    {
      "data": {
        "type": "ghost_busters",
        "id": "Egon Spengler",
        "attributes": {
          "quotes": [
            "I collect spores, molds, and fungus."
          ],
        }
      }
    }

We can map it into a `GhostBuster` class like this:

    public class GhostBuster {
       public string Id { get; set; }
       public List<string> Quotes { get; set; }
    }

In this example, the `JsonApiPropertyResolver` will resolve the `"quotes"` attribute into the `Quotes` property and then proceed to deserialize the quotes into a `List<string>`.

> Note: This is using `Json.NET` under the hood, so you can map complex objects from your `"attributes"` values (or apply a custom `JsonConverter` to them, etc.)!


Property Resolution for Relationships
---
This works exactly the same as Attribute resolution, only you need to use the `[JsonApiRelationship("name")]` attribute instead of `[JsonApiAttribute("name"}]` to map them.

For example, let's say we want to map the `"author"` relationship into a property named `WrittenBy`:

    public class Article
    {
        [JsonApiRelationship("author")]
        public Person WrittenBy { get; set; }
    }
    

Type Resolution for Relationships
---
This **does not** work the same way as attribute types. Resource types are resolved using the `JsonApiTypeResolver`, therefore, you must ensure that the `Type` of the property that your relationships resolve to can hold an instance of the `Type` resolved by the `JsonApiTypeResolver` for the relationship's resource.

Using the _Compound Document_ example from above, this **will not work**:

    public class Article {
        [JsonApiRelationship("author")]
        public Comment Author { get; set; }
    }
    
This doesn't work because the `"type"` for the `"author"` relationship is `"people"`, and the `JsonApiTypeResolver` will resolve this to the `Person` class. The new `Person` instance will then be assigned to the `Author` property (which we've purposefully typed as a `Comment`), resulting in a run-time exception. 


Property Resolution for Id and Type
---
If you want to map the `"id"` and `"type"` fields of the resource into your class, you can use the `[JsonApiId]` or `[JsonApiType]` attributes respectively:

    public class Article
    {
        [JsonApiId]
        public Guid Identifier { get; set; }

        [JsonApiType]
        public string ResourceType { get; set; }
    }
    
   
For the `"id"` field, `JsonApiNet` supports any `Type` that implements a static `Parse(string)` method and will use it to coerce the value found in the document to the appropriate type.


Type Resolution for Resources
---
How does `JsonApiNet` know which class to instantiate for a given resource `"type"`? Enter the `JsonApiTypeResolver`.

When you call `ResourceFromDocument<T>`, you get back an instance of type `T`; however, that is *not* the `Type` used to instantiate your object.

For example, this works:

    var article = JsonApiNet.ResourceFromDocument<object>(json);
    Assert.AreEqual("JSON API paints my bikeshed!", ((Article)article).Title);
    
Notice that the object `Type` is still `Article` (it casts successfully); however, I gave `ResourceFromDocument<T>` the `object` type.

The `JsonApiTypeResolver` is responsible for determining the correct class to instantiate based on the `"type"` attribute associated with the resource in the JSON API document.

> The default resolution process is: 
> 
> 1. First look in the caller's assembly for classes with the `[JsonApiResourceType("name")]` attribute, where `"name"` matches the `"type"` from the document. If one is found, use that class.
> 
> 2. Next, look in the caller's assembly for classes with a name that matches a normalized version of the `"type"` from the document. The normalization applies `Underscore()`, `Singularize()`, and `PascalCase()` to the `"type"`, e.g. `"crazy-cats"` becomes `CrazyCat`

If for some reason you can't apply the `[JsonApiResourceType]` attribute to your classes, you can also write a custom `IJsonApiTypeResolver` that implements whatever mapping you deem fit:

    public class BarneyTypeResolver : IJsonApiTypeResolver {
      public Type ResolveType(string typeName) {
        if (typeName == "rain_drops") {
          return typeof(LemonDrop);
        }
        
        return null;
      }
    }

Then you can use it like so:

    var lemonDrop = JsonApiNet.ResourceFromDocument<LemonDrop>(
        json, 
        new SerializerSettings {
            TypeResolver = new BarneyTypeResolver()
        }
    );

If the `JsonApiTypeResolver` can't find a resource type, it will throw a `JsonApiTypeNotFoundException` 


JsonApiDocument
---
In addition to providing lots of awesome ways to get concrete resource classes out of your JSON API documents, `JsonApiNet` can also get you an object graph that represents the entire JSON API document.

This is an instance of `JsonApiDocument` and has all the good stuff like `Meta`, `Links`, `Errors`, etc parsed into it. For example, if there were `'links'` at the top-level of a document:

    {
      "links": {
        "admin": "http://admin.to/articles"
      },
      "data": { ... }
    }

We can deserialize into a `JsonApiDocument` and fetch the `'admin'` link:

    var document = JsonApiNet.DeseralizeDocument(json);
    var links = document.Links;
    var adminUrl = links["admin"].Uri;
    
In this case, `links` would be a `JsonApiLinks` container instance, holding `JsonApiLink` members. 
    
> Note: Each `JsonApiLink` stored in the `JsonApiLinks` container has `Href` and `Meta` properties for fetching the values parsed out of the document and an additional `Uri` helper that converts the `Href` value into a `System.Uri`. In the case of a "simple link" (i.e., a string containing the link's URL), the `Meta` property will be null.

You can get the parsed representation of the resource (a `JsonApiResource` instance) by calling `document.Data`. This instance will let you fetch the `Attributes`, `Relationships`, `Links`, `Id`, `Type`, and `Meta` that were parsed from the document.

All of the properties are named consistently with the specification, so it shouldn't be too hard to discover what you need to drill down to a specific part of the document.

There is a generic from of `DeserializeDocument<T>` that returns a `JsonApiDocument<T>` instance, which you can use to access the `Resource` property and get what you would have normally received calling `ResourceFromDocument<T>`. For example:

    var document = JsonApiNet.DeserializeDocument<Article>(json);
    var article = document.Resource;
    Assert.AreEqual("JSON API paints my bikeshed!", article.Title);


Errors
---
If you are calling `ResourceFromDocument<T>` then the presence of a top-level `"errors"` field will cause `JsonApiNet` to throw a `JsonApiErrorsException` and stop processing the document. You can get the parsed `JsonApiErrors` instance from the `JsonApiErrors` property on the exception.

Otherwise, a top-level `"errors"` field will be parsed into the `Errors` property of the `JsonApiDocument` and it would be up to you to check for the presence of these in your parsed document and handle them appropriately. There is a convenience property named `HasErrors` on the top level document for this purpose.

For example, consider the following response with errors:

    {
      "errors": [{
        "id": "8c0f0cef-7141-4da4-ad4a-8bbfff2a81a6",
        "status": 500,
        "title": "Error Title",
        "detail": "Error details go here.",
        "meta": {
          "timestamp": "2015-07-23T07:04:32.987Z",
          "path": "/foo/b",
          "trace": [
            "pf_common_api (2.0.4) lib/pf_common_api/errors/helpers.rb:22:in `render_error'",
            "lib/compressed_requests_middleware.rb:23:in `call'"
          ]
        }
      }]
    }

If you were calling `ResourceFromDocument<T>` you might handle the error like this:

    Article article;
    try {
        article = JsonApiNet.ResourceFromDocument<Article>(json);
    } catch (JsonApiErrorsException e) {
        Console.Error.WriteLine("Wat?! {0}", e.JsonApiErrors.Message);
        throw;
    }
    
Alternatively, if you were using `DeserializeDocument`, you might handle it like this:

    var document = JsonApiNet.DeserializeDocument<Article>(json);
    
    if (document.HasErrors) { 
        Console.Error.WriteLine("Wat?! {0}", document.Errors.Message);
        return;
    }

Either of these would give you an output like:

    Wat?! PF::Common::API::Errors::RoutingError: Invalid route


Errata
---

### Deserializing into a Type only known at run-time ###
If you want to deserialize into a container `Type` that is only known at run-time, you can use the non-generic form of `ResourceFromDocument` or `DeserializeDocument` and pass a `ResourceContainerType` value into the `SerializerSettings`: 

    var article = JsonApiNet.ResourceFromDocument(
        json,
        new SerializerSettings {
            ResourceContainerType = typeof(Article)
        }
    );

    Assert.AreEqual("JSON API paints my bikeshed!", ((Article)article).Title);


### Dependency Injection ###
The static methods provided on `JsonApiNet` are convenience methods. You can also instantiate a `JsonApiNetSerializer` yourself and call the similarly-named methods on it. For example:

    var serializer = new JsonApiNetSerializer();
    var article = serializer.ResourceFromDocument<Article>(json);
    Assert.AreEqual("JSON API paints my bikeshed!", article.Title);
    
The `JsonApinetSerializer` inherits from `IJsonApiNetSerializer` which can inject or mock as you see fit.


Contributing
---
Please fork and submit pull requests. If you aren't sure how to approach a fix, or you want some advice, please feel free to open an issue. No contribution is too small.