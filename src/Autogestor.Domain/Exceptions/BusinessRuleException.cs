namespace Autogestor.Domain.Exceptions;

public sealed class BusinessRuleException : DomainException
{
    public BusinessRuleException()
        : base(message: "Violação de regra de negócio.")
    {
    }

    public BusinessRuleException(string message)
        : base(message: message)
    {
    }

    public BusinessRuleException(string message, Exception innerException)
        : base(message: message, innerException: innerException)
    {
    }
}
