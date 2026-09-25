using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses.Categories;

namespace Autogestor.Application.UseCases.Categories.Commands.CreateCategory;

public interface ICreateCategoryUseCase
{
    Task<CategoryResponse> ExecuteAsync(
        CreateCategoryRequest request,
        CancellationToken cancellationToken = default);
}
