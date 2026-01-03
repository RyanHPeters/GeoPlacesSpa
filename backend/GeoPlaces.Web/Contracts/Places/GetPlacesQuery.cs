namespace GeoPlaces.Web.Contracts.Places;

public record GetPlacesQuery(
    int Page = 1,
    int PageSize = 25,
    string? Category = null,
    string? Q = null,
    string? Sort = null,
    string? Order = null
);
