using System.ServiceModel;
using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Categories;

namespace Autogestor.Contract.Services;

[ServiceContract]
public interface ICategoryService
{
    [OperationContract]
    Task<Response<CategoryResponse?>> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
    [OperationContract]
    Task<Response<DeleteResponse?>> DeleteAsync(DeleteCategoryRequest request, CancellationToken cancellationToken = default);
    [OperationContract]
    Task<PagedResponse<CategoryResponse?>> GetAllAsync(GetAllCategoriesRequest request, CancellationToken cancellationToken = default);
    [OperationContract]
    Task<Response<CategoryResponse?>> GetByIdAsync(GetCategoryByIdRequest request, CancellationToken cancellationToken = default);
    [OperationContract]
    Task<Response<CategoryResponse?>> UpdateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken = default);
}
