namespace GeoPlaces.Contracts.Places;

public record NearbyPlaceDto(
    Guid Id,
    string Name,
    string Category,
    double? DistanceMeters
);
