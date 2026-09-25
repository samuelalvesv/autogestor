using Autogestor.Application.UseCases.Categories.Commands.CreateCategory;
using Autogestor.Application.UseCases.Categories.Commands.DeleteCategory;
using Autogestor.Application.UseCases.Categories.Commands.UpdateCategory;
using Autogestor.Application.UseCases.Categories.Queries.GetAllCategories;
using Autogestor.Application.UseCases.Categories.Queries.GetCategoryById;
using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Categories;
using Autogestor.Contract.Services;

namespace Autogestor.Api.Services;

public sealed class CategoryService(
    ICreateCategoryUseCase createCategoryUseCase,
    IDeleteCategoryUseCase deleteCategoryUseCase,
    IGetAllCategoriesUseCase getAllCategoriesUseCase,
    IGetCategoryByIdUseCase getCategoryByIdUseCase,
    IUpdateCategoryUseCase updateCategoryUseCase) : ICategoryService
{
    public Task<CategoryResponse> CreateAsync(
        CreateCategoryRequest request,
        CancellationToken cancellationToken = default) =>
        createCategoryUseCase.ExecuteAsync(
            request: request,
            cancellationToken: cancellationToken);

    public Task<DeleteResponse> DeleteAsync(
        DeleteCategoryRequest request,
        CancellationToken cancellationToken = default) =>
        deleteCategoryUseCase.ExecuteAsync(
            request: request,
            cancellationToken: cancellationToken);

    public Task<PagedResponse<CategoryResponse>> GetAllAsync(
        GetAllCategoriesRequest request,
        CancellationToken cancellationToken = default) =>
        getAllCategoriesUseCase.ExecuteAsync(
            request: request,
            cancellationToken: cancellationToken);

    public Task<CategoryResponse> GetByIdAsync(
        GetCategoryByIdRequest request,
        CancellationToken cancellationToken = default) =>
        getCategoryByIdUseCase.ExecuteAsync(
            request: request,
            cancellationToken: cancellationToken);

    public Task<CategoryResponse> UpdateAsync(
        UpdateCategoryRequest request,
        CancellationToken cancellationToken = default) =>
        updateCategoryUseCase.ExecuteAsync(
            request: request,
            cancellationToken: cancellationToken);
}
