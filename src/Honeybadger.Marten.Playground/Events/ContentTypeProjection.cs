using Marten.Events.Aggregation;

namespace Honeybadger.Marten.Playground.Events;

sealed class ContentTypeProjection : SingleStreamProjection<ContentType, Guid>
{
    public static ContentType Create(ContentTypeCreated created) => new() { Id = created.ContentTypeId, Name = created.Name, Fields = created.Fields };
    public static ContentType Apply(FieldsAdded added, ContentType type)
    {
        type.Fields.Add(added.Name);
        return type;
    }

    public static ContentType Apply(FieldsRemoved removed, ContentType type)
    {
        type.Fields.Remove(removed.Name);
        return type;
    }
}