using System.Net;
using System.Net.Http.Json;
using GeoPlaces.Contracts.Location;
using Microsoft.AspNetCore.Http;

namespace GeoPlaces.Infrastructure.Services;

public interface IGeoIpService
{
    Task<MyLocationDto> GetLocationForRequestAsync(HttpContext httpContext, CancellationToken ct);
}

public class GeoIpService : IGeoIpService
{
    // NYC fallback (Times Square-ish)
    private const double NycLat = 40.7580;
    private const double NycLng = -73.9855;

    private readonly HttpClient _http;

    public GeoIpService(HttpClient http)
    {
        _http = http;
    }

    public async Task<MyLocationDto> GetLocationForRequestAsync(HttpContext httpContext, CancellationToken ct)
    {
        var ip = GetClientIp(httpContext);

        if (ip is null || IPAddress.IsLoopback(ip))
        {
            return new MyLocationDto("fallback-localhost-nyc", ip?.ToString(), NycLat, NycLng, "New York", "NY", "US");
        }

        // ipapi: https://ipapi.co/<ip>/json/ :contentReference[oaicite:2]{index=2}
        var url = $"https://ipapi.co/{ip}/json/";

        IpApiResponse? resp;
        try
        {
            resp = await _http.GetFromJsonAsync<IpApiResponse>(url, cancellationToken: ct);
        }
        catch
        {
            // Any failure: fall back to NYC to keep the demo moving
            return new MyLocationDto("fallback-geoip-failed-nyc", ip.ToString(), NycLat, NycLng, "New York", "NY", "US");
        }

        if (resp?.latitude is null || resp.longitude is null)
        {
            return new MyLocationDto("fallback-geoip-missing-nyc", ip.ToString(), NycLat, NycLng, "New York", "NY", "US");
        }

        return new MyLocationDto(
            "geoip-ipapi",
            ip.ToString(),
            resp.latitude.Value,
            resp.longitude.Value,
            resp.city,
            resp.region,
            resp.country
        );
    }

    private static IPAddress? GetClientIp(HttpContext ctx)
    {
        // If you're behind a proxy, you can also enable ForwardedHeaders middleware.
        // For now, parse X-Forwarded-For if present:
        if (ctx.Request.Headers.TryGetValue("X-Forwarded-For", out var xff))
        {
            var first = xff.ToString().Split(',').FirstOrDefault()?.Trim();
            if (IPAddress.TryParse(first, out var parsed))
                return parsed;
        }

        return ctx.Connection.RemoteIpAddress;
    }

    private sealed class IpApiResponse
    {
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public string? city { get; set; }
        public string? region { get; set; }
        public string? country { get; set; }
    }
}
