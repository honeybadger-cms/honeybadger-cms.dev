namespace Honeybadger.Domain.ContentType;

/// <summary>
/// Repository interface for content type registration and lookup.
/// </summary>
public interface IContentTypeRepository
{
    Task<bool> ExistsAsync(ContentTypeName name, CancellationToken cancellationToken = default);
    Task RegisterAsync(ContentType contentType, CancellationToken cancellationToken = default);
}
