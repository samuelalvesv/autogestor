namespace Autogestor.Domain.Exceptions;

public sealed class DomainValidationException : DomainException
{
    public DomainValidationException()
        : base(message: "Dados inválidos.", errorType: DomainErrorType.Validation)
    {
    }

    public DomainValidationException(string message)
        : base(message: message, errorType: DomainErrorType.Validation)
    {
    }

    public DomainValidationException(string message, Exception innerException)
        : base(message: message, innerException: innerException, errorType: DomainErrorType.Validation)
    {
    }
}
