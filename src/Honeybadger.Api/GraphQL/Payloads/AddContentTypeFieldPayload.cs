using Honeybadger.Api.GraphQL.Types;

namespace Honeybadger.Api.GraphQL.Payloads;

public class AddContentTypeFieldPayload
{
    public required string Name { get; set; }
    public required DatabaseDataType Type { get; set; }
}