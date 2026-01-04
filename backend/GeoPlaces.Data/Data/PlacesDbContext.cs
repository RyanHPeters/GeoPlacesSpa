using GeoPlaces.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoPlaces.Data;

public class PlacesDbContext : DbContext
{
    public DbSet<PlaceEntity> Places => Set<PlaceEntity>();

    public PlacesDbContext(DbContextOptions<PlacesDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<PlaceEntity>();
        entity.ToTable("places");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Id).HasColumnName("id");
        entity.Property(x => x.Name).HasColumnName("name");
        entity.Property(x => x.Category).HasColumnName("category");
        entity.Property(x => x.Location).HasColumnName("location");
        entity.Property(x => x.CreatedAt).HasColumnName("created_at");
    }
}
