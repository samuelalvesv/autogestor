using Autogestor.Application.Interfaces;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Interfaces;

namespace Autogestor.Application.UseCases.Transactions.Commands.DeleteTransaction;

public sealed class DeleteTransactionUseCase(
    ITransactionRepository transactionRepository,
    IUnitOfWork unitOfWork) : IDeleteTransactionUseCase
{
    public async Task<Response<DeleteResponse>> ExecuteAsync(
        DeleteTransactionRequest request,
        CancellationToken cancellationToken = default)
    {
        Transaction? transaction = await transactionRepository.GetByIdAsync(
            id: request.Id,
            cancellationToken: cancellationToken);

        if (transaction is null)
            return new Response<DeleteResponse>
            {
                Data = null,
                Message = "Transação não encontrada."
            };

        await transactionRepository.RemoveAsync(transaction: transaction, cancellationToken: cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

        return new Response<DeleteResponse>
        {
            Data = new DeleteResponse
            {
                Id = transaction.Id
            },
            Message = "Transação excluída com sucesso."
        };
    }
}
