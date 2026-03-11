namespace PristaneLaverieSmart.API.Contracts;

public sealed record ApiErrorResponse(
    string Title,
    int Status,
    string? Detail = null,
    IDictionary<string, string[]>? Errors = null,
    string? TraceId = null
);