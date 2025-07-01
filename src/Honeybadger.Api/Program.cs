using Honeybadger.Api;
using Honeybadger.Api.Data.Abstractions;
using Honeybadger.Api.Data.Repository;
using Honeybadger.Api.GraphQL.Schema.Mutations;
using Honeybadger.Api.GraphQL.Schema.Queries;
using Honeybadger.Api.GraphQL.Types;

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
