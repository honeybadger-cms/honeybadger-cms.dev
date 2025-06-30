namespace Honeybadger.Domain.ContentType;

/// <summary>
/// Value object for content type name, encapsulates validation and rules.
/// </summary>
public sealed class ContentTypeName
{
    public string Value { get; }

    private ContentTypeName(string value)
    {
        Value = value;
    }

    public static ContentTypeName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Content type name cannot be null or empty.", nameof(value));
        if (!System.Text.RegularExpressions.Regex.IsMatch(value, "^[a-zA-Z][a-zA-Z0-9_]*$"))
            throw new ArgumentException("Invalid content type name.", nameof(value));
        return new ContentTypeName(value);
    }

    public override bool Equals(object? obj)
        => obj is ContentTypeName other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
