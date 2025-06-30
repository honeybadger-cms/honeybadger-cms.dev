using Honeybadger.Api.GraphQL.Types;

namespace Honeybadger.Api.GraphQL.Inputs;

public class AddContentTypeInput
{
    public required string Name { get; set; }
    public required List<ContentTypeFieldInput> Fields { get; set; } = [];
}
