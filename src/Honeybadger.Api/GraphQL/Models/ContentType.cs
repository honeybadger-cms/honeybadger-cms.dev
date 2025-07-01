namespace Honeybadger.Api.GraphQL.Models;

public sealed class ContentType
{
    public string Name { get; set; } = string.Empty;
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }
}
