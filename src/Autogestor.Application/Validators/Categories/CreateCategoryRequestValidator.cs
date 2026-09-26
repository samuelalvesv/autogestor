using Autogestor.Contract.Requests.Categories;
using FluentValidation;

namespace Autogestor.Application.Validators.Categories;

public sealed class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(expression: static x => x.Title)
            .NotEmpty().WithMessage(errorMessage: "O título da categoria não pode ser vazio.")
            .Length(min: 3, max: 80).WithMessage(errorMessage: "O título deve conter entre 3 e 80 caracteres.");

        RuleFor(expression: static x => x.Description)
            .NotEmpty().WithMessage(errorMessage: "A descrição da categoria não pode ser vazia.")
            .Length(min: 3, max: 180).WithMessage(errorMessage: "A descrição deve conter entre 3 e 180 caracteres.");
    }
}
