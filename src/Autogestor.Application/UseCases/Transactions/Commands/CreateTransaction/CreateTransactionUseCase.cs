using Autogestor.Application.Interfaces;
using Autogestor.Contract.Enums;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses.Transactions;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Exceptions;
using Autogestor.Domain.Interfaces;

namespace Autogestor.Application.UseCases.Transactions.Commands.CreateTransaction;

public sealed class CreateTransactionUseCase(
    ITransactionRepository transactionRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : ICreateTransactionUseCase
{
    public async Task<TransactionResponse> ExecuteAsync(
        CreateTransactionRequest request,
        CancellationToken cancellationToken = default)
    {
        bool categoryExists = await categoryRepository.ExistsAsync(
            id: request.CategoryId,
            cancellationToken: cancellationToken);

        if (!categoryExists)
            throw new NotFoundException(message: "Categoria não encontrada para o tenant atual.");

        var transaction = Transaction.Create(
            title: request.Title,
            type: (Domain.Enums.ETransactionType)request.Type,
            amount: request.Amount,
            categoryId: request.CategoryId);

        transactionRepository.Add(transaction: transaction);
        await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

        return new TransactionResponse
        {
            Id = transaction.Id,
            Active = transaction.Active,
            CreatedBy = transaction.CreatedBy,
            CreatedAt = transaction.CreatedAt,
            UpdatedBy = transaction.UpdatedBy,
            UpdatedAt = transaction.UpdatedAt,
            TenantId = transaction.TenantId,
            Title = transaction.Title,
            Type = (ETransactionType)transaction.Type,
            Amount = transaction.Amount,
            CategoryId = transaction.CategoryId
        };
    }
}
