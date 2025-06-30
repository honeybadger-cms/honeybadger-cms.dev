using Honeybadger.Api.GraphQL.Resolvers;

namespace Honeybadger.Api.GraphQL.Schemas
{
    public class MutationType : ObjectType<ContentTypeMutation>
    {
        protected override void Configure(IObjectTypeDescriptor<ContentTypeMutation> descriptor)
        {
            descriptor.Field(f => f.CreateContentTypeAsync(default!))
                .Description("Registers a new content type and creates its table.");
        }
    }
}
