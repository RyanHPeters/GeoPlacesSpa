using GeoPlaces.Contracts.Events;
using GeoPlaces.Web.Data;
using GeoPlaces.Web.Application.Places;
using GeoPlaces.Web.Data.Repositories;
using GeoPlaces.Web.Infrastructure;
using GeoPlaces.Web.Services;
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

app.MapFallbackToFile("index.html", new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.ContentRootPath, "wwwroot", "browser"))
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllers();
app.Run();
