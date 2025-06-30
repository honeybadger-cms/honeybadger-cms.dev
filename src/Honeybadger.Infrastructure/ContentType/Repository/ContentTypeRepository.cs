using Dapper;
using Honeybadger.Domain.ContentType.Inputs;
using Honeybadger.Domain.ContentType.Payloads;
using Honeybadger.Domain.ContentType.Types;
using Npgsql;

namespace Honeybadger.Infrastructure.ContentType.Repository;

public sealed class ContentTypeRepository : IContentTypeRepository
{
    private readonly string _connectionString;

    public ContentTypeRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        var exists = await connection.ExecuteScalarAsync<bool>(
            "SELECT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = @name)",
            new { name = name.ToLower() });
        return exists;
    }

    public async Task<AddContentTypePayload> RegisterAsync(AddContentTypeInput contentType, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        try
        {
            var insertSql = @"INSERT INTO cms_content_types (id, name, table_name, created_at) VALUES (@Id, @Name, @TableName, @CreatedAt)";
            await connection.ExecuteAsync(insertSql, new
            {
                Id = Guid.NewGuid(),
                contentType.Name,
                TableName = contentType.Name.ToLower(),
                CreatedAt = DateTime.UtcNow
            });

            var columns = new List<string> { "\"Id\" UUID PRIMARY KEY DEFAULT gen_random_uuid()" };
            foreach (var field in contentType.Fields)
            {
                var col = $"\"{field.Name}\" {MapToPostgresType(field.Type)}";
                columns.Add(col);
            }
            var createTableSql = $"CREATE TABLE IF NOT EXISTS \"{contentType.Name.ToLower()}\" ({string.Join(", ", columns)})";
            var result = await connection.ExecuteAsync(createTableSql);
            if (result == 0)
            {
                return AddContentTypePayload.Error($"Failed to create table for content type '{contentType.Name}'.");
            }
            return AddContentTypePayload.Success(contentType);
        }
        catch (Exception ex)
        {
            return AddContentTypePayload.Error($"An error occurred while registering content type: {ex.Message}");
        }
    }

    private static string MapToPostgresType(DatabaseType type)
    {
        return type switch
        {
            DatabaseType.TEXT => "TEXT",
            DatabaseType.INTEGER => "INTEGER",
            DatabaseType.BOOLEAN => "BOOLEAN",
            DatabaseType.DECIMAL => "DECIMAL",
            DatabaseType.SMALLINT => "SMALLINT",
            DatabaseType.BIGINT => "BIGINT",
            DatabaseType.TIMESTAMP => "TIMESTAMP",
            _ => "TEXT"
        };
    }
}
