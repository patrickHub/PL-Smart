namespace PristaneLaverieSmart.Application.Common.Exceptions;


public sealed class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors {get;}

    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or More validation errors occured")
    {
        Errors = errors;
    }
}