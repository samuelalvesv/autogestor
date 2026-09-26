using Autogestor.Contract.Requests.Categories;
using FluentValidation;

namespace Autogestor.Application.Validators.Categories;

public sealed class DeleteCategoryRequestValidator : AbstractValidator<DeleteCategoryRequest>
{
    public DeleteCategoryRequestValidator()
    {
        RuleFor(expression: static x => x.Id)
            .NotEmpty().WithMessage(errorMessage: "O identificador da categoria é obrigatório.");
    }
}
