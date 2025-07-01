using Honeybadger.Domain.ContentType.Types;

namespace Honeybadger.Domain.ContentType.Inputs;

public class AddContentTypeFieldInput
{
    public required string Name { get; set; }
    public required DatabaseType Type { get; set; }
}