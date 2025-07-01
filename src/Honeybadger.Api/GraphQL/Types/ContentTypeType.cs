using Honeybadger.Api.GraphQL.Models;

namespace Honeybadger.Api.GraphQL.Types;

public class ContentTypeType: ObjectType<ContentType>
{
    protected override void Configure(IObjectTypeDescriptor<ContentType> descriptor)
    {
        descriptor.Field(ct => ct.Name)
            .Type<StringType>();
    }
}
