using Autogestor.Contract.Requests.Categories;
using FluentValidation;

namespace Autogestor.Application.Validators.Categories;

public sealed class GetCategoryByIdRequestValidator : AbstractValidator<GetCategoryByIdRequest>
{
    public GetCategoryByIdRequestValidator()
    {
        RuleFor(expression: static x => x.Id)
            .NotEmpty().WithMessage(errorMessage: "O identificador da categoria é obrigatório.");
    }
}
