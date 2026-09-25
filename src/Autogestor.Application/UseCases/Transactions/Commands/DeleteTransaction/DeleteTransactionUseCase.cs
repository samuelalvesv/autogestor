using Autogestor.Application.Interfaces;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Exceptions;
using Autogestor.Domain.Interfaces;

namespace Autogestor.Application.UseCases.Transactions.Commands.DeleteTransaction;

public sealed class DeleteTransactionUseCase(
    ITransactionRepository transactionRepository,
    IUnitOfWork unitOfWork) : IDeleteTransactionUseCase
{
    public async Task<DeleteResponse> ExecuteAsync(
        DeleteTransactionRequest request,
        CancellationToken cancellationToken = default)
    {
        Transaction? transaction = await transactionRepository.GetByIdAsync(
            id: request.Id,
            cancellationToken: cancellationToken)
            ?? throw new NotFoundException(message: "Transação não encontrada.");

        transactionRepository.Remove(transaction: transaction);
        await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

        return new DeleteResponse
        {
            Id = transaction.Id
        };
    }
}
