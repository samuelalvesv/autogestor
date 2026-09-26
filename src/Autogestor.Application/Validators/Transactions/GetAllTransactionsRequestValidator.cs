using Autogestor.Contract;
using Autogestor.Contract.Requests.Transactions;
using FluentValidation;

namespace Autogestor.Application.Validators.Transactions;

public sealed class GetAllTransactionsRequestValidator : AbstractValidator<GetAllTransactionsRequest>
{
    public GetAllTransactionsRequestValidator()
    {
        RuleFor(expression: static x => x.PageSize)
            .InclusiveBetween(from: ContractDefaults.MinPageSize, to: ContractDefaults.MaxPageSize)
            .WithMessage(errorMessage: $"O tamanho da página deve estar entre {ContractDefaults.MinPageSize} e {ContractDefaults.MaxPageSize}.");
    }
}
