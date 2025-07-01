namespace Honeybadger.Domain.ContentType.Inputs;

public class AddContentTypeInput
{
    public required string Name { get; set; }
    public required List<AddContentTypeFieldInput> Fields { get; set; } = [];
}
