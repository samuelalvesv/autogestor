using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Categories;

namespace Autogestor.Application.UseCases.Categories.Reads.GetCategoryById;

public interface IGetCategoryByIdUseCase
{
    Task<Response<CategoryResponse>> ExecuteAsync(
        GetCategoryByIdRequest request,
        CancellationToken cancellationToken = default);
}
