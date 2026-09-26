using Autogestor.Application.Mappers;
using Autogestor.Application.Validators;
using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses.Categories;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Exceptions;
using Autogestor.Domain.Interfaces;
using FluentValidation;

namespace Autogestor.Application.UseCases.Categories.Queries.GetCategoryById;

public sealed class GetCategoryByIdUseCase(
    ICategoryRepository categoryRepository,
    IValidator<GetCategoryByIdRequest> validator) : IGetCategoryByIdUseCase
{
    public async Task<CategoryResponse> ExecuteAsync(
        GetCategoryByIdRequest request,
        CancellationToken cancellationToken = default)
    {
        await validator.ValidateOrThrowAsync(instance: request, cancellationToken: cancellationToken);

        Category? category = await categoryRepository.GetByIdAsync(
            id: request.Id,
            asNoTracking: true,
            cancellationToken: cancellationToken)
            ?? throw new NotFoundException(message: "Categoria não encontrada.");

        return CategoryMapper.ToResponse(category: category);
    }
}
