using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Categories;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Interfaces;

namespace Autogestor.Application.UseCases.Categories.Queries.GetAllCategories;

public sealed class GetAllCategoriesUseCase(
    ICategoryRepository categoryRepository) : IGetAllCategoriesUseCase
{
    public async Task<PagedResponse<CategoryResponse>> ExecuteAsync(
        GetAllCategoriesRequest request,
        CancellationToken cancellationToken = default)
    {
        (IReadOnlyList<Category> categories, int count) = await categoryRepository.GetPagedAsync(
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            cancellationToken: cancellationToken);

        if (categories.Count == 0)
            return new PagedResponse<CategoryResponse>
            {
                Data = null,
                Message = "Categorias não encontradas.",
                TotalCount = count,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

        IReadOnlyList<CategoryResponse> response = [.. categories
            .Select(category => new CategoryResponse
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
            })];

        return new PagedResponse<CategoryResponse>
        {
            Data = response,
            Message = "Categorias encontradas com sucesso.",
            TotalCount = count,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
