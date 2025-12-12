namespace GeoPlaces.Web.Models;

public record MyLocationDto(
    string Source,
    string? Ip,
    double Latitude,
    double Longitude,
    string? City = null,
    string? Region = null,
    string? Country = null
);
