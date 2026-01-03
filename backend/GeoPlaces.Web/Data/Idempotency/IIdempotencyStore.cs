namespace GeoPlaces.Web.Data.Idempotency;

public record IdempotencyRecord(string Key, string RequestHash, int StatusCode, string ResponseBodyJson);

public interface IIdempotencyStore
{
    Task<IdempotencyRecord?> GetAsync(string key, CancellationToken ct);
    Task SaveAsync(IdempotencyRecord record, CancellationToken ct);
}
