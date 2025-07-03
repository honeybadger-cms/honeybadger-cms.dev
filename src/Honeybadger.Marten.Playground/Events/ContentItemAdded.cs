namespace Honeybadger.Marten.Playground.Events;

public sealed record ContentItemAdded(Guid ContentItemId, Guid ContentTypeId, Dictionary<string, object> Fields);
