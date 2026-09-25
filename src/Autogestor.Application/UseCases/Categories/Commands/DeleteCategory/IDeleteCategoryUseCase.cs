using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses;

namespace Autogestor.Application.UseCases.Categories.Commands.DeleteCategory;

public interface IDeleteCategoryUseCase
{
    Task<DeleteResponse> ExecuteAsync(
        DeleteCategoryRequest request,
        CancellationToken cancellationToken = default);
}
