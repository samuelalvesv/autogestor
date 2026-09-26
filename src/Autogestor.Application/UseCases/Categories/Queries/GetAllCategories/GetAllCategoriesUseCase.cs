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
        (IReadOnlyList<Category>? categories, bool hasNextPage) = await categoryRepository.GetPagedAsync(
            cursor: request.Cursor,
            pageSize: request.PageSize,
            cancellationToken: cancellationToken);

        IReadOnlyList<CategoryResponse> response = [.. categories
            .Select(selector: static category => new CategoryResponse
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
            HasNextPage = hasNextPage,
            NextCursor = hasNextPage ? categories[^1].Id : null
        };
    }
}
