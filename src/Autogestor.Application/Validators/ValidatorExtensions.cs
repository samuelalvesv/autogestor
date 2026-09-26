using Autogestor.Domain.Exceptions;
using FluentValidation;
using FluentValidation.Results;

namespace Autogestor.Application.Validators;

public static class ValidatorExtensions
{
    public static async Task ValidateOrThrowAsync<T>(
        this IValidator<T> validator,
        T instance,
        CancellationToken cancellationToken = default)
    {
        ValidationResult result = await validator.ValidateAsync(instance: instance, cancellation: cancellationToken);
        if (!result.IsValid)
        {
            string message = string.Join(separator: ", ", values: result.Errors.Select(selector: static e => e.ErrorMessage));
            throw new DomainValidationException(message: message);
        }
    }
}
