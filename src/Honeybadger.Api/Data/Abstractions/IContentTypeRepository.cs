using Honeybadger.Api.GraphQL.Inputs;
using Honeybadger.Api.GraphQL.Models;
using Honeybadger.Api.GraphQL.Payloads;

namespace Honeybadger.Api.Data.Abstractions
{
    /// <summary>
    /// Repository interface for content type registration and lookup.
    /// </summary>
    public interface IContentTypeRepository
    {
        Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);
        Task<AddContentTypePayload> RegisterAsync(AddContentTypeInput contentType, CancellationToken cancellationToken = default);
        Task<ContentType> GetContentTypeAsync(string name, CancellationToken cancellationToken = default);
    }
}
