namespace Autogestor.Domain.Exceptions;

public sealed class DomainValidationException : DomainException
{
    public DomainValidationException()
        : base(message: "Dados inválidos.")
    {
    }

    public DomainValidationException(string message)
        : base(message: message)
    {
    }

    public DomainValidationException(string message, Exception innerException)
        : base(message: message, innerException: innerException)
    {
    }
}
