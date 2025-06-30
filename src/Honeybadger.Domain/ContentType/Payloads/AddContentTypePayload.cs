using Honeybadger.Domain.ContentType.Inputs;

namespace Honeybadger.Domain.ContentType.Payloads;

public sealed record class AddContentTypePayload
{
    public string Name { get; set; } = string.Empty;
    public IReadOnlyList<ContentTypeFieldOutput> Fields { get; set; } = [];
    public string ErrorMessage { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
    public bool ContentTypeAdded { get; set; }

    public static AddContentTypePayload Error(string message)
    {
        return new AddContentTypePayload
        {
            ErrorMessage = message,
            ContentTypeAdded = false,
        };
    }

    public static AddContentTypePayload Success(AddContentTypeInput input)
    {
        var fields = input.Fields.Select(f => new ContentTypeFieldOutput
        {
            Name = f.Name,
            Type = f.Type,
        }).ToList();
        return new AddContentTypePayload
        {
            Name = input.Name,
            Fields = fields,
            ContentTypeAdded = true,
            CreatedAt = DateTime.UtcNow
        };
    }

}
