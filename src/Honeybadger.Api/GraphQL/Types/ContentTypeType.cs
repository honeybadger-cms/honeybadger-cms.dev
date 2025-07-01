using Honeybadger.Api.GraphQL.Models;

namespace Honeybadger.Api.GraphQL.Types;

public class ContentTypeType: ObjectType<ContentType>
{
    protected override void Configure(IObjectTypeDescriptor<ContentType> descriptor)
    {
        descriptor.Field(ct => ct.Name)
            .Type<StringType>();

        descriptor.Field(ct => ct.CreatedAt)
            .Name("created_at")
            .Description("The date and time when the content type was created.")
            .Type<DateTimeType>();

        descriptor.Field(ct => ct.Id)
            .Type<NonNullType<UuidType>>()
            .Description("The unique identifier for the content type.");
    }
}
