namespace Honeybadger.Domain.ContentType;

public sealed class ContentTypeFieldDefinition
{
    public string Name { get; }
    public PostgresFieldType Type { get; }
    public bool IsNullable { get; }
    public bool IsUnique { get; }
    public int? MaxLength { get; }
    public int? MinLength { get; }

    public ContentTypeFieldDefinition(
        string name,
        PostgresFieldType type,
        bool isNullable = true,
        bool isUnique = false,
        int? maxLength = null,
        int? minLength = null)
    {
        Name = name;
        Type = type;
        IsNullable = isNullable;
        IsUnique = isUnique;
    }
}
