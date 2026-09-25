namespace Autogestor.Domain.Exceptions;

public sealed class ConflictException : DomainException
{
    public ConflictException()
        : base(message: "Recurso já existente.", errorType: DomainErrorType.Conflict)
    {
    }

    public ConflictException(string message)
        : base(message: message, errorType: DomainErrorType.Conflict)
    {
    }

    public ConflictException(string message, Exception innerException)
        : base(message: message, innerException: innerException, errorType: DomainErrorType.Conflict)
    {
    }
}
