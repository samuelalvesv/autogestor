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
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            cancellationToken: cancellationToken);

        if (transactions.Count == 0)
            return new PagedResponse<TransactionResponse>
            {
                Data = null,
                Message = "Transações não encontradas.",
                TotalCount = count,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

        IReadOnlyList<TransactionResponse> response = [.. transactions
            .Select(transaction => new TransactionResponse
            {
                Id = transaction.Id,
                Active = transaction.Active,
                CreatedBy = transaction.CreatedBy,
                CreatedAt = transaction.CreatedAt,
                UpdatedBy = transaction.UpdatedBy,
                UpdatedAt = transaction.UpdatedAt,
                TenantId = transaction.TenantId,
                Title = transaction.Title,
                Type = (Contract.Enums.ETransactionType)transaction.Type,
                Amount = transaction.Amount,
                CategoryId = transaction.CategoryId
            })];

        return new PagedResponse<TransactionResponse>
        {
            Data = response,
            Message = "Transações encontradas com sucesso.",
            TotalCount = count,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
