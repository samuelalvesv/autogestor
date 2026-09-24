using Autogestor.Application.UseCases.Categories.Commands.CreateCategory;
using Autogestor.Application.UseCases.Categories.Commands.DeleteCategory;
using Autogestor.Application.UseCases.Categories.Commands.UpdateCategory;
using Autogestor.Application.UseCases.Categories.Queries.GetCategoryById;
using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Categories;
using Autogestor.Contract.Services;

namespace Autogestor.Api.Services;

public sealed class CategoryService(
    ICreateCategoryUseCase createCategoryUseCase,
    IDeleteCategoryUseCase deleteCategoryUseCase,
    IGetCategoryByIdUseCase getCategoryByIdUseCase,
    IUpdateCategoryUseCase updateCategoryUseCase) : ICategoryService
{
    public Task<Response<CategoryResponse>> CreateAsync(
        CreateCategoryRequest request,
        CancellationToken cancellationToken = default) =>
        createCategoryUseCase.ExecuteAsync(
            request: request,
            cancellationToken: cancellationToken);

    public Task<Response<DeleteResponse>> DeleteAsync(
        DeleteCategoryRequest request,
        CancellationToken cancellationToken = default) =>
        deleteCategoryUseCase.ExecuteAsync(
            request: request,
            cancellationToken: cancellationToken);

    public Task<PagedResponse<CategoryResponse>> GetAllAsync(
        GetAllCategoriesRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(result: new PagedResponse<CategoryResponse>
        {
            Data = [],
            Message = "Implementação pendente.",
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        });

    public Task<Response<CategoryResponse>> GetByIdAsync(
        GetCategoryByIdRequest request,
        CancellationToken cancellationToken = default) =>
        getCategoryByIdUseCase.ExecuteAsync(
            request: request,
            cancellationToken: cancellationToken);

    public Task<Response<CategoryResponse>> UpdateAsync(
        UpdateCategoryRequest request,
        CancellationToken cancellationToken = default) =>
        updateCategoryUseCase.ExecuteAsync(
            request: request,
            cancellationToken: cancellationToken);
}
