using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Transactions;

namespace Autogestor.Application.UseCases.Transactions.Queries.GetTransactionById;

public interface IGetTransactionByIdUseCase
{
    Task<Response<TransactionResponse>> ExecuteAsync(
        GetTransactionByIdRequest request,
        CancellationToken cancellationToken = default);
}
