using Autogestor.Application.Interfaces;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Transactions;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Interfaces;

namespace Autogestor.Application.UseCases.Transactions.Reads.GetTransactionById;

public sealed class GetTransactionByIdUseCase(
    ITransactionRepository transactionRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IGetTransactionByIdUseCase
{
    public async Task<Response<TransactionResponse>> ExecuteAsync(
        GetTransactionByIdRequest request,
        CancellationToken cancellationToken = default)
    {
        Transaction? transaction = await transactionRepository.GetByIdAsync(
            id: request.Id,
            cancellationToken: cancellationToken);

        if (transaction is null)
            return new Response<TransactionResponse>
            {
                Data = null,
                Message = "Transação não encontrada."
            };

        var response = new TransactionResponse
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
        };

        return new Response<TransactionResponse>
        {
            Data = response,
            Message = "Transação encontrada com sucesso."
        };
    }
}
