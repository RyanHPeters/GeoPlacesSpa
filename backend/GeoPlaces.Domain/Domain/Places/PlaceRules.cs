namespace GeoPlaces.Domain.Places;

public static class PlaceRules
{
    public static void ValidateNewPlace(string name, string category, GeoPoint point)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (name.Length > 200)
            throw new ArgumentException("Name must be 200 characters or fewer.", nameof(name));

        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category is required.", nameof(category));
        if (category.Length > 100)
            throw new ArgumentException("Category must be 100 characters or fewer.", nameof(category));

        if (point.Latitude is < -90 or > 90)
            throw new ArgumentException("Latitude must be between -90 and 90.", nameof(point));
        if (point.Longitude is < -180 or > 180)
            throw new ArgumentException("Longitude must be between -180 and 180.", nameof(point));
    }
}
