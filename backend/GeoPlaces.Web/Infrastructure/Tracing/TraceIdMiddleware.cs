using System.Diagnostics;

namespace GeoPlaces.Web.Infrastructure.Tracing;

public sealed class TraceIdMiddleware
{
    private const string HeaderName = "trace-id";
    private readonly RequestDelegate _next;

    public TraceIdMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context)
    {
        // Prefer Activity ID when available; fallback to ASP.NET trace identifier
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = traceId;
            return Task.CompletedTask;
        });

        await _next(context);
    }
}
