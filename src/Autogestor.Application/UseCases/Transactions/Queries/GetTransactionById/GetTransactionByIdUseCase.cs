using Autogestor.Contract.Enums;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses.Transactions;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Exceptions;
using Autogestor.Domain.Interfaces;

namespace Autogestor.Application.UseCases.Transactions.Queries.GetTransactionById;

public sealed class GetTransactionByIdUseCase(
    ITransactionRepository transactionRepository) : IGetTransactionByIdUseCase
{
    public async Task<TransactionResponse> ExecuteAsync(
        GetTransactionByIdRequest request,
        CancellationToken cancellationToken = default)
    {
        Transaction? transaction = await transactionRepository.GetByIdAsync(
            id: request.Id,
            asNoTracking: true,
            cancellationToken: cancellationToken)
            ?? throw new NotFoundException(message: "Transação não encontrada.");

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
