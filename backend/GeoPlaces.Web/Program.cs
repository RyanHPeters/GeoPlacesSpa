using Asp.Versioning;
using GeoPlaces.Contracts.Events;
using GeoPlaces.Web.Application.Idempotency;
using GeoPlaces.Web.Application.Places;
using GeoPlaces.Web.Data;
using GeoPlaces.Web.Data.Idempotency;
using GeoPlaces.Web.Data.Repositories;
using GeoPlaces.Web.Infrastructure;
using GeoPlaces.Web.Infrastructure.Errors;
using GeoPlaces.Web.Infrastructure.Tracing;
using GeoPlaces.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<IGeoIpService, GeoIpService>();

builder.Services.AddScoped<IPlaceRepository, PlaceRepositoryEf>();
builder.Services.AddScoped<IPlaceSpatialRepository, PlaceSpatialRepositoryDapper>();
builder.Services.AddScoped<IPlacesService, PlacesService>();

// ProblemDetails + exception handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Idempotency DI
builder.Services.AddScoped<IIdempotencyStore, IdempotencyStoreDapper>();
builder.Services.AddScoped<IIdempotencyService, IdempotencyService>();

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true; // Uses default if none provided
    options.ReportApiVersions = true; // Returns headers (api-supported-versions)

    // Choose how to read the version (Query string, Header, or URL segment)
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"),
        new QueryStringApiVersionReader("api-version")
    );
})
.AddMvc() // Required for Controller support
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // Formats version as 'v1', 'v1.1', etc.
    options.SubstituteApiVersionInUrl = true;
});

// Standardize ProblemDetails and include traceid 
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var traceId = System.Diagnostics.Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;

        var errors = context.ModelState
            .Where(kvp => kvp.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        var pd = new ProblemDetails
        {
            Type = "https://errors.geoplaces.local/validation",
            Title = "Validation failed",
            Status = StatusCodes.Status400BadRequest,
            Detail = "One or more validation errors occurred.",
            Instance = context.HttpContext.Request.Path
        };

        pd.Extensions["traceId"] = traceId;
        pd.Extensions["errors"] = errors;

        return new BadRequestObjectResult(pd)
        {
            ContentTypes = { "application/problem+json" }
        };
    };
});


builder.Services.AddDbContext<PlacesDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Default"),
        npgsql => npgsql.UseNetTopologySuite());
});

builder.Services.AddScoped<IEventPublisher, ConsoleEventPublisher>();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("dev", p =>
        p.AllowAnyOrigin()
         .AllowAnyHeader()
         .AllowAnyMethod());
});

var app = builder.Build();

app.UseMiddleware<TraceIdMiddleware>();
app.UseExceptionHandler(); // uses GlobalExceptionHandler

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("dev");
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.ContentRootPath, "wwwroot", "browser")),
    RequestPath = ""
});

app.UseRouting();
app.UseAuthorization();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllers();

app.MapFallbackToFile("index.html", new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.ContentRootPath, "wwwroot", "browser"))
});

app.Run();
