using Honeybadger.Domain.ContentType.Types;

namespace Honeybadger.Domain.ContentType.Inputs;

public class ContentTypeFieldInput
{
    public required string Name { get; set; }
    public required DatabaseType Type { get; set; }
}