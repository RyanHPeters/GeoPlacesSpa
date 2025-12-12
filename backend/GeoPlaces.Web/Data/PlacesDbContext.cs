using GeoPlaces.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace GeoPlaces.Web.Data;

public class PlacesDbContext : DbContext
{
    public DbSet<Place> Places => Set<Place>();

    public PlacesDbContext(DbContextOptions<PlacesDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Place>();
        entity.ToTable("places");
        entity.HasKey(x => x.id);
        entity.Property(x => x.Name).HasColumnName("name");
        entity.Property(x => x.Category).HasColumnName("category");
        entity.Property(x => x.Location).HasColumnName("location");
        entity.Property(x => x.CreatedAt).HasColumnName("created_at");
    }
}
