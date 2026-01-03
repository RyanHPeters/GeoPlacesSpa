namespace GeoPlaces.Web.Application.Common;

public record PagedItems<T>(IReadOnlyList<T> Items, long TotalCount);
