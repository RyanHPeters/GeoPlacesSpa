using GeoPlaces.Contracts.Places;
using GeoPlaces.Domain.Places;

namespace GeoPlaces.Application.Places;

public static class PlaceDtoMapping
{
    public static PlaceDto ToDto(Place place)
        => new(
            place.Id,
            place.Name,
            place.Category,
            place.Location.Latitude,
            place.Location.Longitude,
            place.CreatedAt
        );
}
