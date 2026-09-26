using Autogestor.Application.Interfaces;
using Autogestor.Application.Validators;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Exceptions;
using Autogestor.Domain.Interfaces;
using FluentValidation;

namespace Autogestor.Application.UseCases.Transactions.Commands.DeleteTransaction;

public sealed class DeleteTransactionUseCase(
    ITransactionRepository transactionRepository,
    IUnitOfWork unitOfWork,
    IValidator<DeleteTransactionRequest> validator) : IDeleteTransactionUseCase
{
    public async Task<DeleteResponse> ExecuteAsync(
        DeleteTransactionRequest request,
        CancellationToken cancellationToken = default)
    {
        await validator.ValidateOrThrowAsync(instance: request, cancellationToken: cancellationToken);

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
