using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Transactions;

namespace Autogestor.Application.UseCases.Transactions.Queries.GetAllTransactions;

public interface IGetAllTransactionsUseCase
{
    Task<PagedResponse<TransactionResponse>> ExecuteAsync(
        GetAllTransactionsRequest request,
        CancellationToken cancellationToken = default);
}
