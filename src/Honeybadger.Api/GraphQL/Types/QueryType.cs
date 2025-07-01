using Honeybadger.Api.GraphQL.Schema.Queries;

namespace Honeybadger.Api.GraphQL.Types;

public class QueryType : ObjectType<Query>
{
    protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
    {
        descriptor.Field(q => q.GetContentType(default!)).Type<ContentTypeType>();
    }
}
