namespace GeoPlaces.Infrastructure.Paging;

public static class PagingMath
{
    public static int TotalPages(int totalItems, int pageSize)
    {
        if (pageSize <= 0) return 0;
        if (totalItems <= 0) return 0;
        return (totalItems + pageSize - 1) / pageSize;
    }

    public static (int skip, int take) SkipTake(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 1;
        int skip = (page - 1) * pageSize;
        return (skip, pageSize);
    }
}