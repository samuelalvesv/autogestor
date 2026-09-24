using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Categories;

namespace Autogestor.Application.UseCases.Categories.Queries.GetAllCategories;

public interface IGetAllCategoriesUseCase
{
    Task<PagedResponse<CategoryResponse>> ExecuteAsync(
        GetAllCategoriesRequest request,
        CancellationToken cancellationToken = default);
}
