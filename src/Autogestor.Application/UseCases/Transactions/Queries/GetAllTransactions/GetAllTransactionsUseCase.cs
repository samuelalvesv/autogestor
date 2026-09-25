using Autogestor.Contract.Enums;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Transactions;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Interfaces;

namespace Autogestor.Application.UseCases.Transactions.Queries.GetAllTransactions;

public sealed class GetAllTransactionsUseCase(
    ITransactionRepository transactionRepository) : IGetAllTransactionsUseCase
{
    public async Task<PagedResponse<TransactionResponse>> ExecuteAsync(
        GetAllTransactionsRequest request,
        CancellationToken cancellationToken = default)
    {
        (IReadOnlyList<Transaction> transactions, int count) = await transactionRepository.GetPagedAsync(
            skip: request.Skip,
            pageSize: request.PageSize,
            cancellationToken: cancellationToken);

        IReadOnlyList<TransactionResponse> response = [.. transactions
            .Select(selector: static transaction => new TransactionResponse
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
            })];

        return new PagedResponse<TransactionResponse>
        {
            Data = response,
            TotalCount = count,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
