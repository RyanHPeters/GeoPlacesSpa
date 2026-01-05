using GeoPlaces.Contracts.Places;

namespace GeoPlaces.Application.Places;

public static class CreatePlaceValidation
{
    public static IReadOnlyList<string> Validate(CreatePlaceRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add("Name is required.");

        if (request.Name?.Length > 200)
            errors.Add("Name must be 200 characters or less.");

        if (string.IsNullOrWhiteSpace(request.Category))
            errors.Add("Category is required.");

        if (request.Category?.Length > 100)
            errors.Add("Category must be 100 characters or less.");

        if (request.Latitude < -90 || request.Latitude > 90)
            errors.Add("Latitude must be between -90 and 90.");

        if (request.Longitude < -180 || request.Longitude > 180)
            errors.Add("Longitude must be between -180 and 180.");

        return errors;
    }
}
