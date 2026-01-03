using GeoPlaces.Web.Application.Errors;
using GeoPlaces.Web.Data.Idempotency;
using GeoPlaces.Web.Infrastructure.Idempotency;

namespace GeoPlaces.Web.Application.Idempotency;

public interface IIdempotencyService
{
    Task<IdempotencyRecord?> TryGetReplayAsync(string key, object request, CancellationToken ct);
    Task SaveResultAsync(string key, object request, int statusCode, object responseBody, CancellationToken ct);
}

public sealed class IdempotencyService : IIdempotencyService
{
    private readonly IIdempotencyStore _store;

    public IdempotencyService(IIdempotencyStore store) => _store = store;

    public async Task<IdempotencyRecord?> TryGetReplayAsync(string key, object request, CancellationToken ct)
    {
        var existing = await _store.GetAsync(key, ct);
        if (existing is null) return null;

        var requestHash = RequestHashing.Hash(request);
        if (!string.Equals(existing.RequestHash, requestHash, StringComparison.Ordinal))
        {
            throw new ConflictException("Idempotency-Key has already been used with a different request payload.");
        }

        return existing;
    }

    public async Task SaveResultAsync(string key, object request, int statusCode, object responseBody, CancellationToken ct)
    {
        var requestHash = RequestHashing.Hash(request);
        var responseJson = System.Text.Json.JsonSerializer.Serialize(responseBody);

        var record = new IdempotencyRecord(key, requestHash, statusCode, responseJson);
        await _store.SaveAsync(record, ct);
    }
}
