using System.ServiceModel;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Transactions;

namespace Autogestor.Contract.Services;

[ServiceContract]
public interface ITransactionService
{
    [OperationContract]
    Task<Response<TransactionResponse>> CreateAsync(CreateTransactionRequest request, CancellationToken cancellationToken = default);

    [OperationContract]
    Task<Response<DeleteResponse>> DeleteAsync(DeleteTransactionRequest request, CancellationToken cancellationToken = default);

    [OperationContract]
    Task<PagedResponse<TransactionResponse>> GetAllAsync(GetAllTransactionsRequest request, CancellationToken cancellationToken = default);

    [OperationContract]
    Task<Response<TransactionResponse>> GetByIdAsync(GetTransactionByIdRequest request, CancellationToken cancellationToken = default);

    [OperationContract]
    Task<Response<TransactionResponse>> UpdateAsync(UpdateTransactionRequest request, CancellationToken cancellationToken = default);
}
