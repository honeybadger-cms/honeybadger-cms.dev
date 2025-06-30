using Honeybadger.Domain.ContentType.Inputs;
using Honeybadger.Domain.ContentType.Payloads;

namespace Honeybadger.Infrastructure.ContentType.Repository
{
    /// <summary>
    /// Repository interface for content type registration and lookup.
    /// </summary>
    public interface IContentTypeRepository
    {
        Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);
        Task<AddContentTypePayload> RegisterAsync(AddContentTypeInput contentType, CancellationToken cancellationToken = default);
    }
}
