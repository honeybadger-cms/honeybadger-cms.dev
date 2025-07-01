using Honeybadger.Api.Data.Abstractions;
using Honeybadger.Api.GraphQL.Models;

namespace Honeybadger.Api.GraphQL.Schema.Queries;

public sealed class Query(IContentTypeRepository repository)
{
    public async Task<ContentType> GetContentType(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Content type name cannot be null or empty.", nameof(name));
        }
        var contentTypeExists = await repository.ExistsAsync(name);
        if (!contentTypeExists)
        {
            throw new GreenDonut.KeyNotFoundException($"Content type '{name}' does not exist.");
        }
        var contentType = await repository.GetContentTypeAsync(name);
        return contentType;
    }
}
