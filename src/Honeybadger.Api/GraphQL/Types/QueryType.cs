using Honeybadger.Api.Data.Abstractions;

namespace Honeybadger.Api.GraphQL.Types;

public class QueryType : ObjectType
{
    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Field("contentType")
            .Argument("name", a => a.Type<NonNullType<StringType>>())
            .Type<ContentTypeType>()
            .Resolve(async (context, ct) =>
            {
                List<string> selectedFields = context.Selection.SyntaxNode.SelectionSet?.Selections
                    .OfType<HotChocolate.Language.FieldNode>()
                    .Select(f => f.Name.Value)
                    .ToList() ?? [];
                var repository = context.Service<IContentTypeRepository>();
                var name = context.ArgumentValue<string>("name");
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new GraphQLException("Content type name cannot be null or empty.");
                }
                var contentTypeExists = await repository.ExistsAsync(name, ct);
                if (!contentTypeExists)
                {
                    throw new GreenDonut.KeyNotFoundException($"Content type '{name}' does not exist.");
                }
                var contentType = await repository.GetContentTypeAsync(name, selectedFields, ct);
                return contentType;
            });
    }
}
