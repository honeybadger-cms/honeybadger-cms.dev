using Dapper;
using Honeybadger.Api.Data.Abstractions;
using Honeybadger.Api.GraphQL.Inputs;
using Honeybadger.Api.GraphQL.Models;
using Honeybadger.Api.GraphQL.Payloads;
using Honeybadger.Api.GraphQL.Types;
using Npgsql;
using System.Text;

namespace Honeybadger.Api.Data.Repository;

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

    public async Task<ContentType> GetContentTypeAsync(string name, List<string> columnNames, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        try
        {
            if (columnNames != null && columnNames.Count > 0)
            {
                StringBuilder sb = new();
                sb.Append("SELECT ");
                sb.Append(string.Join(", ", columnNames.Select(c => $"{c}")));
                sb.Append(" FROM cms_content_types WHERE table_name = @name fetch first 1 rows only");
                var selectSql = sb.ToString();
                var contentType = await connection.QuerySingleOrDefaultAsync<ContentType>(selectSql, new { name = name.ToLower() }) ?? throw new GreenDonut.KeyNotFoundException($"Content type '{name}' does not exist.");
                return contentType;
            }
            else
            {
                throw new GraphQLException("Column names cannot be null or empty.");
            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    private static string MapToPostgresType(DatabaseDataType type)
    {
        return type switch
        {
            DatabaseDataType.TEXT => "TEXT",
            DatabaseDataType.INTEGER => "INTEGER",
            DatabaseDataType.BOOLEAN => "BOOLEAN",
            DatabaseDataType.DECIMAL => "DECIMAL",
            DatabaseDataType.SMALLINT => "SMALLINT",
            DatabaseDataType.BIGINT => "BIGINT",
            DatabaseDataType.TIMESTAMP => "TIMESTAMP",
            _ => "TEXT"
        };
    }
}
