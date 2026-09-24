using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses;

namespace Autogestor.Application.UseCases.Transactions.Commands.DeleteTransaction;

public interface IDeleteTransactionUseCase
{
    Task<Response<DeleteResponse>> ExecuteAsync(
        DeleteTransactionRequest request,
        CancellationToken cancellationToken = default);
}
