using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses;

namespace Autogestor.Application.UseCases.Categories.Commands.DeleteCategory;

public interface IDeleteCategoryUseCase
{
    Task<Response<DeleteResponse>> ExecuteAsync(
        DeleteCategoryRequest request,
        CancellationToken cancellationToken = default);
}
