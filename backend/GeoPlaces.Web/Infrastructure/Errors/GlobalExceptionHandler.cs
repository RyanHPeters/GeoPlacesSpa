using System.Diagnostics;
using GeoPlaces.Web.Application.Errors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GeoPlaces.Web.Infrastructure.Errors;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        if (exception is ApiException apiEx)
        {
            _logger.LogWarning(exception, "Handled API exception. traceId={TraceId}", traceId);

            var pd = CreateProblemDetails(httpContext, apiEx, traceId);

            // Field-level errors for validation
            if (apiEx is ValidationException vex && vex.Errors.Count > 0)
            {
                pd.Extensions["errors"] = vex.Errors;
            }

            httpContext.Response.StatusCode = apiEx.StatusCode;
            httpContext.Response.ContentType = "application/problem+json";
            await httpContext.Response.WriteAsJsonAsync(pd, cancellationToken);

            return true;
        }

        // Fallback 500
        _logger.LogError(exception, "Unhandled exception. traceId={TraceId}", traceId);

        var generic = new ProblemDetails
        {
            Type = "https://errors.geoplaces.local/internal",
            Title = "Internal Server Error",
            Status = StatusCodes.Status500InternalServerError,
            Detail = "An unexpected error occurred.",
            Instance = httpContext.Request.Path
        };
        generic.Extensions["traceId"] = traceId;

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(generic, cancellationToken);

        return true;
    }

    private static ProblemDetails CreateProblemDetails(HttpContext ctx, ApiException ex, string traceId)
    {
        var pd = new ProblemDetails
        {
            Type = ex.Type,
            Title = ex.Title,
            Status = ex.StatusCode,
            Detail = ex.Message,
            Instance = ctx.Request.Path
        };
        pd.Extensions["traceId"] = traceId;
        return pd;
    }
}
