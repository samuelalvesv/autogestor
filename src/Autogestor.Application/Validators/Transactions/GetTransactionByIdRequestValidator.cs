using Autogestor.Contract.Requests.Transactions;
using FluentValidation;

namespace Autogestor.Application.Validators.Transactions;

public sealed class GetTransactionByIdRequestValidator : AbstractValidator<GetTransactionByIdRequest>
{
    public GetTransactionByIdRequestValidator()
    {
        RuleFor(expression: static x => x.Id)
            .NotEmpty().WithMessage(errorMessage: "O identificador da transação é obrigatório.");
    }
}
