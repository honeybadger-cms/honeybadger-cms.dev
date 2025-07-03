namespace Honeybadger.Marten.Playground.Events;

public sealed record ContentTypeCreated(Guid ContentTypeId, string Name, List<string> Fields);
