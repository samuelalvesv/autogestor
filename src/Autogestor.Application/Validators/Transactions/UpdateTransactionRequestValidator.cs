using Autogestor.Contract.Requests.Transactions;
using FluentValidation;

namespace Autogestor.Application.Validators.Transactions;

public sealed class UpdateTransactionRequestValidator : AbstractValidator<UpdateTransactionRequest>
{
    public UpdateTransactionRequestValidator()
    {
        RuleFor(expression: static x => x.Id)
            .NotEmpty().WithMessage(errorMessage: "O identificador da transação é obrigatório.");

        RuleFor(expression: static x => x.Title)
            .NotEmpty().WithMessage(errorMessage: "O título da transação não pode ser vazio.")
            .Length(min: 3, max: 80).WithMessage(errorMessage: "O título deve conter entre 3 e 80 caracteres.");

        RuleFor(expression: static x => x.Type)
            .IsInEnum().WithMessage(errorMessage: "Tipo de transação inválido.");

        RuleFor(expression: static x => x.Amount)
            .GreaterThan(valueToCompare: 0).WithMessage(errorMessage: "O valor da transação deve ser maior que zero.");

        RuleFor(expression: static x => x.CategoryId)
            .NotEmpty().WithMessage(errorMessage: "O identificador da categoria é obrigatório.");
    }
}
