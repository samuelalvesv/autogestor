using Autogestor.Contract;
using Autogestor.Contract.Requests.Categories;
using FluentValidation;

namespace Autogestor.Application.Validators.Categories;

public sealed class GetAllCategoriesRequestValidator : AbstractValidator<GetAllCategoriesRequest>
{
    public GetAllCategoriesRequestValidator()
    {
        RuleFor(expression: static x => x.PageSize)
            .InclusiveBetween(from: ContractDefaults.MinPageSize, to: ContractDefaults.MaxPageSize)
            .WithMessage(errorMessage: $"O tamanho da página deve estar entre {ContractDefaults.MinPageSize} e {ContractDefaults.MaxPageSize}.");
    }
}
