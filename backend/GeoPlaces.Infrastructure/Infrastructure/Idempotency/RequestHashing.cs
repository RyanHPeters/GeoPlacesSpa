using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace GeoPlaces.Infrastructure.Idempotency;

public static class RequestHashing
{
    public static string Hash(object request)
    {
        // Stable JSON to hash (keeps it simple; you can use canonicalization later if needed)
        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(json));
        return Convert.ToHexString(bytes);
    }
}
