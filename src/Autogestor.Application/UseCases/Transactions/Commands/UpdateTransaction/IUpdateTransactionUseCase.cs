using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Transactions;

namespace Autogestor.Application.UseCases.Transactions.Commands.UpdateTransaction;

public interface IUpdateTransactionUseCase
{
    Task<Response<TransactionResponse>> ExecuteAsync(
        UpdateTransactionRequest request,
        CancellationToken cancellationToken = default);
}
