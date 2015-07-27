JsonApiNet
---
A C# library for deserialization of JSON API responses, built on top of [Json.NET](https://github.com/JamesNK/Newtonsoft.Json). 

This library was written against **v1.0** of [the JSON API specification](http://jsonapi.org/format/)

> If you're looking for a serializer for your API, give the [JsonApiMediaTypeFormatter](https://github.com/rmichela/JsonApiMediaTypeFormatter) library a try.


Basic Usage
---
Here is a simple JSON API document representing a single article. The article has an id, a title, and a URL where you can fetch the article resource.

    {
      "data": {
        "type": "articles",
        "id": "30cd428f-1a3b-459b-a9a8-0ca87c14dd31",
        "attributes": {
          "title": "JSON API paints my bikeshed!"
        },
        "links": {
          "self": "http://example.com/articles/11"
        }
      }
    }

Let's say we want to store the article from the document in the following class:

    public class Article
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }
    
The first thing we need to do is parse the document into a `JsonApiDocument<Article>`:

    var document = JsonConvert.DeserializeObject<JsonApiDocument<Article>>(json);

Now we can get the `Resource` property, which will be of type `Article`:

    var article = document.Resource;
    Assert.AreEqual("JSON API paints my bikeshed!", article.Title);

> Note: The `Id` field for our `Article` class above is a `Guid` instead of a `string`. For `id` values, the converter supports any `Type` that implements a static `Parse(string)` method and will attempt to coerce the value found in the document to the appropriate type.


Lists of resources
---

Let's make our sample document return a list with 1 element instead of a single resource:

    {
      "data": [{
        "type": "articles",
        "id": "30cd428f-1a3b-459b-a9a8-0ca87c14dd31",
        "attributes": {
          "title": "JSON API paints my bikeshed!"
        },
        "links": {
          "self": "http://example.com/articles/11"
        }
      }]
    }

Parsing it and getting to our `List<Article>` instance is easy:

    var document = JsonConvert.DeserializeObject<JsonApiDocument<List<Article>>>(json);
    var articles = document.Resource;
    Assert.AreEqual("JSON API paints my bikeshed!", articles[0].Title);


Compound Documents
---
Now let's study a more complex document, with included resources, relationships, and meta data.

This document is a list of articles. The article has a relationship to a single author and a list of comments. The resource data for each relationship is `included` in the document.

    {
      "data": [{
        "type": "articles",
        "id": "1",
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

Here are the objects we'd like to retrieve from this document:

The `Article` class:

    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Person Author { get; set; }
        public List<Comment> Comments { get; set; }
    }

A `Person` class for the author:

    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Twitter { get; set; }
    }

And a `Comment` class for the comments:

    public class Comment
    {
        public int Id { get; set; }
        public string Body { get; set; }
    }

Nothing changes about how we get the list of articles:

    var document = JsonConvert.DeserializeObject<JsonApiDocument<List<Article>>>(json);
    var articles = document.Resource;
    Assert.AreEqual("JSON API paints my bikeshed!", articles[0].Title);

And look, the author and comments are available too:

    var author = articles[0].Author;
    Assert.AreEqual("dgeb", author.Twitter);

    var comments = articles[0].Comments;
    Assert.AreEqual("I like XML better", comments[1].Body);

> Note: If you need access to additional data about the relationships, you need to map the appropriate fields into your concrete classes (see the _Advanced Usage_ section) or drill down via the `document` instance to the `Relationships` and `Included` methods (see the _Additional Response Data_ section).


Additional Response Data
---

You can also query the `JsonApiDocument` instance for the full structure of the parsed document, including `Meta`, `Links`, `Errors`, etc.

For example, if there were `'links'` at the top-level of the document:

    {
      "links": {
        "admin": "http://admin.to/articles"
      },
      "data": { ... }
    }

Then we can fetch the `'admin'` link:

    var links = document.Links;
    var adminUrl = links["admin"].Uri;
    
In this case, `links` would be a `JsonApiLinks` container instance, holding `JsonApiLink` members. 
    
> Note: Each `JsonApiLink` stored in the `JsonApiLinks` container has `Href` and `Meta` properties for fetching the values parsed out of the document and an additional `Uri` helper that converts the `Href` value into a `System.Uri`. In the case of a "simple link" (i.e., a string containing the link's URL), the `Meta` property will be null.

You can get the parsed representation of the resource by calling `document.Data`, which is a `JsonApiResource` instance. This instance will let you fetch the `Attributes`, `Relationships`, `Links`, `Id`, `Type`, and `Meta` that were parsed from the document.


Advanced Usage
---
In the _Basic Usage_ section, you might have noticed that there is a case mismatch between the `title` attribute in the document and the `Title` field on the `Article` class.

That is because by default, `JsonApi.Data` will `Underscore()` and then `Pascalize()` the attributes keys (using [Humanizer](https://github.com/MehdiK/Humanizer)) and then map your document's attributes into the resulting property name (i.e., `first-name` becomes `FirstName`).

You can override this behavior by explicitly setting the `[JsonApiAttribute("title")]` attribute on one or more fields in your container objects.

For example, we could map the `title` to a field named `originalTitle`

    public class Article
    {
        [JsonApiAttribute("title")]
        public string originalTitle { get; set; }
    }

> Note: It is technically allowed by the spec to name different attributes in the same resource object with keys that differ only in case or style. 
>
>     "data": { 
>       "attributes": { 
>         "first-name": "Alpha", 
>         "first_name": "Beta", 
>         "firstName": "Charlie" 
>       }
>     }
>     
> If you do this (really?) and you don't also set explicitly set a `[JsonApiAttribute("")]` value for each of them, the behavior of this library with respect to those attributes is undefined.

Similarly, you can map relationships using the `[JsonApiRelationship]` attribute:

    public class Article
    {
        [JsonApiRelationship("author")]
        public Person WrittenBy { get; set; }
    }

> Note: Fields for a resource object **MUST** share a common namespace with each other and with `type` and `id`. In other words, a resource can not have an attribute and relationship with the same name, nor can it have an attribute or relationship named `type` or `id`. This library is non-validating, so it does not attempt to enforce this. 

If you want to map the `Id` or `Type` fields, you can use the `[JsonApiId]` or `[JsonApiType]` attributes respectively:

    public class Article
    {
        [JsonApiId]
        public Guid Identifier { get; set; }

        [JsonApiType]
        public string ResourceType { get; set; }
    }

Finally, if your `attributes` contains complex objects that are not included resources, they will be converted using the standard Json.NET converter, and so you can map their property names via `JsonProperty`, etc.


Errors
---

A top-level `errors` key will be parsed into the `Errors` property of the `document`. It is up to you to check for errors in your document and handle them appropriately. There is a convenience property named `HasErrors` on the top level document.

For example, let's say the document looks like this:

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

You could handle it and print an error message like so:

    var document = JsonConvert.DeserializeObject<JsonApiDocument>(json);
    
    if (document.HasErrors)
    { 
        Console.Error.WriteLine("Wat?! {0}", document.Errors.Message);
    }
    else
    {
        // ...
    }

This would give you the output:

    Wat?! PF::Common::API::Errors::RoutingError: Invalid route


Contributing
---
Please fork and submit pull requests. If you aren't sure how to approach a fix, or you want some advice, please feel free to open an issue. No contribution is too small.