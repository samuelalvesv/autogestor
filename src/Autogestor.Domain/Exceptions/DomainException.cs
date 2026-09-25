namespace Autogestor.Domain.Exceptions;

public abstract class DomainException : Exception
{
    public DomainErrorType ErrorType { get; }

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

    protected DomainException(string message, DomainErrorType errorType)
        : base(message: message)
    {
        ErrorType = errorType;
    }

    protected DomainException(string message, Exception innerException, DomainErrorType errorType)
        : base(message: message, innerException: innerException)
    {
        ErrorType = errorType;
    }
}
