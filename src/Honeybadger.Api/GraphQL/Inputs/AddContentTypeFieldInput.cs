using Honeybadger.Api.GraphQL.Types;

namespace Honeybadger.Api.GraphQL.Inputs;

public class AddContentTypeFieldInput
{
    public required string Name { get; set; }
    public required DatabaseDataType Type { get; set; }
}