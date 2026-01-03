using Dapper;
using Npgsql;

namespace GeoPlaces.Web.Data.Idempotency;

public sealed class IdempotencyStoreDapper : IIdempotencyStore
{
    private readonly string _connStr;

    public IdempotencyStoreDapper(IConfiguration config)
    {
        _connStr = config.GetConnectionString("Default")
                  ?? throw new InvalidOperationException("Missing Default connection string.");
    }

    public async Task<IdempotencyRecord?> GetAsync(string key, CancellationToken ct)
    {
        const string sql = """
            SELECT key as "Key", request_hash as "RequestHash", status_code as "StatusCode", response_body::text as "ResponseBodyJson"
            FROM idempotency_keys
            WHERE key = @key;
        """;

        await using var conn = new NpgsqlConnection(_connStr);
        return await conn.QuerySingleOrDefaultAsync<IdempotencyRecord>(new CommandDefinition(sql, new { key }, cancellationToken: ct));
    }

    public async Task SaveAsync(IdempotencyRecord record, CancellationToken ct)
    {
        const string sql = """
            INSERT INTO idempotency_keys (key, request_hash, status_code, response_body)
            VALUES (@Key, @RequestHash, @StatusCode, @ResponseBodyJson::jsonb)
            ON CONFLICT (key) DO NOTHING;
        """;

        await using var conn = new NpgsqlConnection(_connStr);
        await conn.ExecuteAsync(new CommandDefinition(sql, record, cancellationToken: ct));
    }
}
