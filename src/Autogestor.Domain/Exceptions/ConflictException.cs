namespace Autogestor.Domain.Exceptions;

public sealed class ConflictException : DomainException
{
    public ConflictException()
        : base(message: "Recurso já existente.")
    {
    }

    public ConflictException(string message)
        : base(message: message)
    {
    }

    public ConflictException(string message, Exception innerException)
        : base(message: message, innerException: innerException)
    {
    }
}
