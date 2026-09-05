using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Categories;

namespace Autogestor.Application.UseCases.Categories.Commands.CreateCategory;

public interface ICreateCategoryUseCase
{
    Task<Response<CategoryResponse>> ExecuteAsync(
        CreateCategoryRequest request,
        CancellationToken cancellationToken = default);
}
