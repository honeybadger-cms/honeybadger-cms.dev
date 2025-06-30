using Honeybadger.Application.ContentType;
using Honeybadger.Domain.ContentType.Inputs;
using Honeybadger.Domain.ContentType.Payloads;
using Honeybadger.Infrastructure.ContentType.Repository;

namespace Honeybadger.Api.GraphQL.Resolvers;

public sealed class ContentTypeMutation(IContentTypeRepository repository)
{
    public async Task<AddContentTypePayload> CreateContentTypeAsync(AddContentTypeInput input)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(input.Name))
            {
                return AddContentTypePayload.Error("Content type name cannot be null or empty.");
            }
            if (input.Fields == null || input.Fields.Count == 0)
            {
                return AddContentTypePayload.Error("At least one field definition is required.");
            }

            var command = new RegisterContentTypeCommand(input);
            var handler = new RegisterContentTypeHandler(repository);
            var result = await handler.HandleAsync(command);
            return result;
        }
        catch (Exception ex)
        {
            return AddContentTypePayload.Error($"An error occurred while creating content type: {ex.Message}");        }
    }
}
