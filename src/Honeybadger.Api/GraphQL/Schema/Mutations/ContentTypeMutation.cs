using Honeybadger.Api.Data.Abstractions;
using Honeybadger.Api.GraphQL.Inputs;
using Honeybadger.Api.GraphQL.Payloads;

namespace Honeybadger.Api.GraphQL.Schema.Mutations;

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

            var result = await repository.ExistsAsync(input.Name.ToLower())
                ? AddContentTypePayload.Error($"Content type '{input.Name}' already exists.")
                : await repository.RegisterAsync(input);
            return result;
        }
        catch (Exception ex)
        {
            return AddContentTypePayload.Error($"An error occurred while creating content type: {ex.Message}");        }
    }
}
