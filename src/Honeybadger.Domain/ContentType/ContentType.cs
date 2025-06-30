namespace Honeybadger.Domain.ContentType;

/// <summary>
/// Domain entity representing a content type.
/// </summary>
public sealed class ContentType
{
    public Guid Id { get; }
    public ContentTypeName Name { get; }
    public DateTime CreatedAt { get; }
    public IReadOnlyList<ContentTypeFieldDefinition> Fields { get; }

    private ContentType(Guid id, ContentTypeName name, DateTime createdAt, IReadOnlyList<ContentTypeFieldDefinition> fields)
    {
        Id = id;
        Name = name;
        CreatedAt = createdAt;
        Fields = fields;
    }

    public static ContentType Register(ContentTypeName name, IReadOnlyList<ContentTypeFieldDefinition> fields)
    {
        return new ContentType(Guid.NewGuid(), name, DateTime.UtcNow, fields);
    }
}
