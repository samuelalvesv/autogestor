using System.ServiceModel;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Transactions;

namespace Autogestor.Contract.Services;

[ServiceContract]
public interface ITransactionService
{
    [OperationContract]
    Task<TransactionResponse> CreateAsync(CreateTransactionRequest request, CancellationToken cancellationToken = default);

    [OperationContract]
    Task<DeleteResponse> DeleteAsync(DeleteTransactionRequest request, CancellationToken cancellationToken = default);

    [OperationContract]
    Task<PagedResponse<TransactionResponse>> GetAllAsync(GetAllTransactionsRequest request, CancellationToken cancellationToken = default);

    [OperationContract]
    Task<TransactionResponse> GetByIdAsync(GetTransactionByIdRequest request, CancellationToken cancellationToken = default);

    [OperationContract]
    Task<TransactionResponse> UpdateAsync(UpdateTransactionRequest request, CancellationToken cancellationToken = default);
}
