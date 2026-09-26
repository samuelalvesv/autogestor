namespace Autogestor.Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException()
    {
    }

    protected DomainException(string message)
        : base(message: message)
    {
    }

    protected DomainException(string message, Exception innerException)
        : base(message: message, innerException: innerException)
    {
    }
}
