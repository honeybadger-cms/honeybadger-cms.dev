using Honeybadger.Domain.ContentType.Types;

namespace Honeybadger.Domain.ContentType.Payloads;

public class AddContentTypeFieldOutput
{
    public required string Name { get; set; }
    public required DatabaseType Type { get; set; }
}