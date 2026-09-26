using Autogestor.Application.Mappers;
using Autogestor.Application.Validators;
using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Categories;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Interfaces;
using FluentValidation;

namespace Autogestor.Application.UseCases.Categories.Queries.GetAllCategories;

public sealed class GetAllCategoriesUseCase(
    ICategoryRepository categoryRepository,
    IValidator<GetAllCategoriesRequest> validator) : IGetAllCategoriesUseCase
{
    public async Task<PagedResponse<CategoryResponse>> ExecuteAsync(
        GetAllCategoriesRequest request,
        CancellationToken cancellationToken = default)
    {
        await validator.ValidateOrThrowAsync(instance: request, cancellationToken: cancellationToken);

        (IReadOnlyList<Category>? categories, bool hasNextPage) = await categoryRepository.GetPagedAsync(
            cursor: request.Cursor,
            pageSize: request.PageSize,
            cancellationToken: cancellationToken);

        return CategoryMapper.ToPagedResponse(
            categories: categories,
            hasNextPage: hasNextPage);
    }
}
