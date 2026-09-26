using Autogestor.Contract.Requests.Transactions;
using FluentValidation;

namespace Autogestor.Application.Validators.Transactions;

public sealed class DeleteTransactionRequestValidator : AbstractValidator<DeleteTransactionRequest>
{
    public DeleteTransactionRequestValidator()
    {
        RuleFor(expression: static x => x.Id)
            .NotEmpty().WithMessage(errorMessage: "O identificador da transação é obrigatório.");
    }
}
