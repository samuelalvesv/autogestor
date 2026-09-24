using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Categories;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Interfaces;

namespace Autogestor.Application.UseCases.Categories.Queries.GetCategoryById;

public sealed class GetCategoryByIdUseCase(
    ICategoryRepository categoryRepository) : IGetCategoryByIdUseCase
{
    public async Task<Response<CategoryResponse>> ExecuteAsync(
        GetCategoryByIdRequest request,
        CancellationToken cancellationToken = default)
    {
        Category? category = await categoryRepository.GetByIdAsync(
            id: request.Id,
            asNoTracking: true,
            cancellationToken: cancellationToken);

        if (category is null)
            return new Response<CategoryResponse>
            {
                Data = null,
                Message = "Categoria não encontrada."
            };

        var response = new CategoryResponse
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

        return new Response<CategoryResponse>
        {
            Data = response,
            Message = "Categoria encontrada com sucesso."
        };
    }
}
