namespace Honeybadger.Marten.Playground.Events;

public sealed record FieldsAdded(Guid FieldId, string Name, string Type);
