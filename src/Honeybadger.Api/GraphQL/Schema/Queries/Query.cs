using Honeybadger.Api.GraphQL.Models;

namespace Honeybadger.Api.GraphQL.Schema.Queries;

public sealed class Query
{
    public string Hello() => "world";

    public ContentType GetContentType(string name)
    {
        // This is a placeholder implementation.
        // In a real application, you would retrieve the content type from a repository or service.
        return new ContentType { Name = name };
    }
}
