using Honeybadger.Marten.Playground.Events;
using JasperFx;
using JasperFx.Events;
using JasperFx.Events.Projections;
using Marten;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddOpenApi();
builder.Services.AddMarten(options =>
{
    options.Connection(connectionString);

    options.UseSystemTextJsonForSerialization();
    options.Events.StreamIdentity = StreamIdentity.AsString;

    if (builder.Environment.IsDevelopment())
    {
        options.AutoCreateSchemaObjects = AutoCreate.All;
    }
});

var store = DocumentStore.For(_ =>
{
    _.Connection(connectionString);
    _.Projections.Add<ContentTypeProjection>(ProjectionLifecycle.Inline);
});

var app = builder.Build();

var contentTypeId = Guid.NewGuid();
await CreateContentTypeAsync(store, contentTypeId);
var contentItemId = Guid.NewGuid();
await CreateContentItemAsync(store, contentItemId, contentTypeId);

await ViewProjectionAsync(store, contentTypeId);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

async static Task CreateContentTypeAsync(IDocumentStore store, Guid contentTypeId)
{
    using var session = store.LightweightSession();
    var evt = new ContentTypeCreated(contentTypeId, "Article", ["Title", "Text", "Author", "CreatedAt"]);

    session.Events.StartStream<ContentType>(contentTypeId, evt);
    await session.SaveChangesAsync();
}

async static Task CreateContentItemAsync(IDocumentStore store,Guid contentItemId, Guid contentTypeId)
{
    using var session = store.LightweightSession();
    var evt = new ContentItemAdded(contentItemId, contentTypeId, new()
    {
        ["Title"] = "Sample Article",
        ["Text"] = "This is a sample article text.",
        ["Author"] = "John Doe",
        ["CreatedAt"] = DateTime.UtcNow
    });

    session.Events.Append(contentTypeId, evt);
    await session.SaveChangesAsync();
}

async static Task ViewProjectionAsync(IDocumentStore store, Guid contentTypeId)
{
    await using var session = store.LightweightSession();
    var fieldAdded = new FieldsAdded(Guid.NewGuid(), "Header", "string");
    var fieldAdded2 = new FieldsAdded(Guid.NewGuid(), "Logo", "string");
    var fieldAdded3 = new FieldsAdded(Guid.NewGuid(), "Image", "string");
    session.Events.Append(contentTypeId, fieldAdded, fieldAdded2, fieldAdded3);
    await session.SaveChangesAsync();

    var contentTypeState = await session.LoadAsync<ContentType>(contentTypeId);

    var fieldRemoved = new FieldsRemoved(Guid.NewGuid(), "Header");
    session.Events.Append(contentTypeId, fieldRemoved);
    await session.SaveChangesAsync();
    contentTypeState = await session.LoadAsync<ContentType>(contentTypeId);
}

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public class ContentType
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public List<string> Fields { get; set; } = ["Title", "Text", "Author", "CreatedAt"];
}
