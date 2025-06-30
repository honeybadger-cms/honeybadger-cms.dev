using Dapper;
using Npgsql;
namespace Honeybadger.Api.GraphQL.Resolvers;

public sealed class Query
{
    public string Hello() => "world";
}
