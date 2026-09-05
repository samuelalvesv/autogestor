using Autogestor.Application.UseCases.Categories.Commands.CreateCategory;
using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Categories;
using Autogestor.Contract.Services;

namespace Autogestor.Api.Services;

public sealed class CategoryService(ICreateCategoryUseCase createCategoryUseCase) : ICategoryService
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
        Task.FromResult(result: new Response<DeleteResponse>
        {
            Data = null,
            Message = "Implementação pendente."
        });

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
        Task.FromResult(result: new Response<CategoryResponse>
        {
            Data = null,
            Message = "Implementação pendente."
        });

    public Task<Response<CategoryResponse>> UpdateAsync(
        UpdateCategoryRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(result: new Response<CategoryResponse>
        {
            Data = null,
            Message = "Implementação pendente."
        });
}
