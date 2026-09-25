namespace Autogestor.Domain.Exceptions;

public sealed class NotFoundException : DomainException
{
    public NotFoundException()
        : base(message: "Recurso não encontrado.", errorType: DomainErrorType.NotFound)
    {
    }

    public NotFoundException(string message)
        : base(message: message, errorType: DomainErrorType.NotFound)
    {
    }

    public NotFoundException(string message, Exception innerException)
        : base(message: message, innerException: innerException, errorType: DomainErrorType.NotFound)
    {
    }
}
