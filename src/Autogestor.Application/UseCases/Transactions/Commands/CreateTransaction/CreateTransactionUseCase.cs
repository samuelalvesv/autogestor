using Autogestor.Application.Interfaces;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Transactions;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Interfaces;

namespace Autogestor.Application.UseCases.Transactions.Commands.CreateTransaction;

public sealed class CreateTransactionUseCase(
    ITransactionRepository transactionRepository,
    IUnitOfWork unitOfWork) : ICreateTransactionUseCase
{
    public async Task<Response<TransactionResponse>> ExecuteAsync(
        CreateTransactionRequest request,
        CancellationToken cancellationToken = default)
    {
        var transaction = Transaction.Create(
            title: request.Title,
            type: (Domain.Enums.ETransactionType)request.Type,
            amount: request.Amount,
            categoryId: request.CategoryId);

        await transactionRepository.AddAsync(
            transaction: transaction,
            cancellationToken: cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

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
            Message = "Transação criada com sucesso."
        };
    }
}
