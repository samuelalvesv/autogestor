namespace Autogestor.Domain.Exceptions;

public sealed class BusinessRuleException : DomainException
{
    public BusinessRuleException()
        : base(message: "Violação de regra de negócio.", errorType: DomainErrorType.BusinessRule)
    {
    }

    public BusinessRuleException(string message)
        : base(message: message, errorType: DomainErrorType.BusinessRule)
    {
    }

    public BusinessRuleException(string message, Exception innerException)
        : base(message: message, innerException: innerException, errorType: DomainErrorType.BusinessRule)
    {
    }
}
