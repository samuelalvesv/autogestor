using Autogestor.Application.Interfaces;
using Autogestor.Application.Mappers;
using Autogestor.Application.Validators;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses.Transactions;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Exceptions;
using Autogestor.Domain.Interfaces;
using FluentValidation;

namespace Autogestor.Application.UseCases.Transactions.Commands.UpdateTransaction;

public sealed class UpdateTransactionUseCase(
    ITransactionRepository transactionRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IValidator<UpdateTransactionRequest> validator) : IUpdateTransactionUseCase
{
    public async Task<TransactionResponse> ExecuteAsync(
        UpdateTransactionRequest request,
        CancellationToken cancellationToken = default)
    {
        await validator.ValidateOrThrowAsync(instance: request, cancellationToken: cancellationToken);

        Transaction? transaction = await transactionRepository.GetByIdAsync(
            id: request.Id,
            cancellationToken: cancellationToken)
            ?? throw new NotFoundException(message: "Transação não encontrada.");

        bool categoryExists = await categoryRepository.ExistsAsync(
            id: request.CategoryId,
            cancellationToken: cancellationToken);

        if (!categoryExists)
            throw new NotFoundException(message: "Categoria não encontrada para o tenant atual.");

        transaction.Update(
            title: request.Title,
            type: TransactionMapper.ToDomain(type: request.Type),
            amount: request.Amount,
            categoryId: request.CategoryId);

        await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

        return TransactionMapper.ToResponse(transaction: transaction);
    }
}
