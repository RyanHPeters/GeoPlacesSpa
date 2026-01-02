namespace GeoPlaces.Web.Contracts.Places;

public record CreatePlaceRequest(
    string Name,
    string Category,
    double Latitude,
    double Longitude
);
