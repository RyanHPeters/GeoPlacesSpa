using Microsoft.AspNetCore.Http;

namespace GeoPlaces.Application.Errors;

public abstract class ApiException : Exception
{
    protected ApiException(string message) : base(message) { }
    public virtual int StatusCode => 500;
    public virtual string Type => "about:blank";
    public virtual string Title => "An error occurred";
}

public sealed class ValidationException : ApiException
{
    public ValidationException(string message, IDictionary<string, string[]>? errors = null) : base(message)
        => Errors = errors ?? new Dictionary<string, string[]>();

    public override int StatusCode => StatusCodes.Status400BadRequest;
    public override string Type => "https://errors.geoplaces.local/validation";
    public override string Title => "Validation failed";
    public IDictionary<string, string[]> Errors { get; }
}

public sealed class NotFoundException : ApiException
{
    public NotFoundException(string message) : base(message) { }
    public override int StatusCode => StatusCodes.Status404NotFound;
    public override string Type => "https://errors.geoplaces.local/not-found";
    public override string Title => "Resource not found";
}

public sealed class ConflictException : ApiException
{
    public ConflictException(string message) : base(message) { }
    public override int StatusCode => StatusCodes.Status409Conflict;
    public override string Type => "https://errors.geoplaces.local/conflict";
    public override string Title => "Conflict";
}
