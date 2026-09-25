using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses.Transactions;

namespace Autogestor.Application.UseCases.Transactions.Commands.CreateTransaction;

public interface ICreateTransactionUseCase
{
    Task<TransactionResponse> ExecuteAsync(
        CreateTransactionRequest request,
        CancellationToken cancellationToken = default);
}
