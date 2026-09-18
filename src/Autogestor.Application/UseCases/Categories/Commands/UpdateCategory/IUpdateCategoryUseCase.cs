using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Categories;

namespace Autogestor.Application.UseCases.Categories.Commands.UpdateCategory;

public interface IUpdateCategoryUseCase
{
    Task<Response<CategoryResponse>> ExecuteAsync(
        UpdateCategoryRequest request,
        CancellationToken cancellationToken = default);
}
