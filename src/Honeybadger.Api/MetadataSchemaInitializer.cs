using Dapper;
using Npgsql;

namespace Honeybadger.Api;

public static class MetadataSchemaInitializer
{
    public static async Task EnsureMetadataTablesExistAsync(WebApplication app, CancellationToken cancellationToken = default)
    {
        var configuration = app.Services.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("DefaultConnection")!; ;
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        var ensureTableSql = @"CREATE TABLE IF NOT EXISTS cms_content_types (
            id UUID PRIMARY KEY,
            name TEXT NOT NULL,
            table_name TEXT NOT NULL,
            created_at TIMESTAMPTZ NOT NULL
        )";
        await connection.ExecuteAsync(ensureTableSql);
    }
}
