namespace GeoPlaces.Domain.Common;

public record PagedItems<T>(IReadOnlyList<T> Items, long TotalCount);
