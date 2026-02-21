using FluentValidation;
using FluentValidation.Results;


namespace PristaneLaverieSmart.Application.Common.Validation;

public static class ValidatorExtensions
{
    public static async Task ValidateAndThrowAsync<T>(this IValidator<T> validator, T instance, CancellationToken ct)
    {
        ValidationResult result = await validator.ValidateAsync(instance, ct);

        if(result.IsValid) return;

        var errors = result.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).Distinct().ToArray()
            );
        
        throw new Exceptions.ValidationException(errors);
    }
}