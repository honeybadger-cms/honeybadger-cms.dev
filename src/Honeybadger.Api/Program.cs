using Honeybadger.Api.GraphQL.Resolvers;
using Honeybadger.Api.GraphQL.Schemas;
using Honeybadger.Infrastructure.ContentType;
using Honeybadger.Infrastructure.ContentType.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IContentTypeRepository>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connStr = config.GetConnectionString("DefaultConnection")!;
    return new ContentTypeRepository(connStr);
});
builder.Services.AddScoped<ContentTypeMutation>();
builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<MutationType>()
    .AddDefaultTransactionScopeHandler();

var app = builder.Build();

await MetadataSchemaInitializer.EnsureMetadataTablesExistAsync(app);

app.UseHttpsRedirection();

app.MapGraphQL();

app.Run();
