namespace GeoPlaces.Contracts.Places;

public record PlaceDto(
    Guid Id,
    string Name,
    string Category,
    double Latitude,
    double Longitude,
    DateTime CreatedAt
);
