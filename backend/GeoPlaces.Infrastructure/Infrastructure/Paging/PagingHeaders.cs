using Microsoft.AspNetCore.WebUtilities;

using Microsoft.AspNetCore.Http;

namespace GeoPlaces.Infrastructure.Paging;

public static class PagingHeaders
{
    public static void AddPaging(HttpResponse response, HttpRequest request, int page, int pageSize, long totalCount)
    {
        response.Headers["X-Total-Count"] = totalCount.ToString();

        var lastPage = (int)Math.Max(1, Math.Ceiling(totalCount / (double)pageSize));

        var links = new List<string>
        {
            BuildLink(request, 1, pageSize, "first"),
            BuildLink(request, lastPage, pageSize, "last")
        };

        if (page > 1) links.Add(BuildLink(request, page - 1, pageSize, "prev"));
        if (page < lastPage) links.Add(BuildLink(request, page + 1, pageSize, "next"));

        response.Headers["Link"] = string.Join(", ", links);
    }

    private static string BuildLink(HttpRequest request, int page, int pageSize, string rel)
    {
        var baseUrl = $"{request.Scheme}://{request.Host}{request.Path}";
        Dictionary<string, string?> query = request.Query.ToDictionary(k => k.Key, v => (string?)v.Value.ToString());

        query["page"] = page.ToString();
        query["pageSize"] = pageSize.ToString();

        var url = QueryHelpers.AddQueryString(baseUrl, query);
        return $"<{url}>; rel=\"{rel}\"";
    }
}
