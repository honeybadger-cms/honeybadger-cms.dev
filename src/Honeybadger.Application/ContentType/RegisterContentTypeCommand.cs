using Honeybadger.Domain.ContentType.Inputs;

namespace Honeybadger.Application.ContentType;

/// <summary>
/// Command for registering a new content type.
/// </summary>
public sealed class RegisterContentTypeCommand(AddContentTypeInput addContentTypeInput)
{
    public AddContentTypeInput ContentTypeInput { get; } = addContentTypeInput ?? throw new ArgumentNullException(nameof(addContentTypeInput), "Content type input cannot be null.");
}
