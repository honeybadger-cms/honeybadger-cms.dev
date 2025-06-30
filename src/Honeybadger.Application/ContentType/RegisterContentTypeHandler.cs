using Honeybadger.Domain.ContentType.Payloads;
using Honeybadger.Infrastructure.ContentType.Repository;

namespace Honeybadger.Application.ContentType;

public sealed class RegisterContentTypeHandler(IContentTypeRepository repository)
{
    public async Task<AddContentTypePayload> HandleAsync(RegisterContentTypeCommand command, CancellationToken cancellationToken = default)
    {
        string name = command.ContentTypeInput.Name;

        if (await repository.ExistsAsync(name, cancellationToken))
        {
            return AddContentTypePayload.Error($"Content type '{name}' already exists.");
        }

        var fieldNames = new HashSet<string>();
        foreach (var field in command.ContentTypeInput.Fields)
        {
            if (!fieldNames.Add(field.Name.ToLower()))
            {
                return AddContentTypePayload.Error($"Duplicate field name: {field.Name}");
            }
            if (string.IsNullOrWhiteSpace(field.Name))
            {
                return AddContentTypePayload.Error("Field name is required.");
            }
        }

        var result = await repository.RegisterAsync(command.ContentTypeInput, cancellationToken);

        return result;
    }
}
