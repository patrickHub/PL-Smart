namespace PristaneLaverieSmart.Application.Common.Exceptions;


public sealed class ValidationException : Exception
{
    public IDictionary<string, string[]> Error {get;}

    public ValidationException(IDictionary<string, string[]> error)
        : base("One or More validation errors occured")
    {
        Error = error;
    }
}