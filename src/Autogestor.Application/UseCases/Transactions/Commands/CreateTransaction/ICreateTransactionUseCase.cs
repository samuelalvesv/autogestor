using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Transactions;

namespace Autogestor.Application.UseCases.Transactions.Commands.CreateTransaction;

public interface ICreateTransactionUseCase
{
    Task<Response<TransactionResponse>> ExecuteAsync(
        CreateTransactionRequest request,
        CancellationToken cancellationToken = default);
}
