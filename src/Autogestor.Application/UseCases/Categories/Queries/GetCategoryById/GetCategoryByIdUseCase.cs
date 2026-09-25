using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses.Categories;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Exceptions;
using Autogestor.Domain.Interfaces;

namespace Autogestor.Application.UseCases.Categories.Queries.GetCategoryById;

public sealed class GetCategoryByIdUseCase(
    ICategoryRepository categoryRepository) : IGetCategoryByIdUseCase
{
    public async Task<CategoryResponse> ExecuteAsync(
        GetCategoryByIdRequest request,
        CancellationToken cancellationToken = default)
    {
        Category? category = await categoryRepository.GetByIdAsync(
            id: request.Id,
            asNoTracking: true,
            cancellationToken: cancellationToken)
            ?? throw new NotFoundException(message: "Categoria não encontrada.");

        return new CategoryResponse
        {
            Id = category.Id,
            Active = category.Active,
            CreatedBy = category.CreatedBy,
            CreatedAt = category.CreatedAt,
            UpdatedBy = category.UpdatedBy,
            UpdatedAt = category.UpdatedAt,
            TenantId = category.TenantId,
            Title = category.Title,
            Description = category.Description
        };
    }
}
