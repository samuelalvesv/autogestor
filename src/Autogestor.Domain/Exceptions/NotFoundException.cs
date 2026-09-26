namespace Autogestor.Domain.Exceptions;

public sealed class NotFoundException : DomainException
{
    public NotFoundException()
        : base(message: "Recurso não encontrado.")
    {
    }

    public NotFoundException(string message)
        : base(message: message)
    {
    }

    public NotFoundException(string message, Exception innerException)
        : base(message: message, innerException: innerException)
    {
    }
}
