using Autogestor.Application.Mappers;
using Autogestor.Application.Validators;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Transactions;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Interfaces;
using FluentValidation;

namespace Autogestor.Application.UseCases.Transactions.Queries.GetAllTransactions;

public sealed class GetAllTransactionsUseCase(
    ITransactionRepository transactionRepository,
    IValidator<GetAllTransactionsRequest> validator) : IGetAllTransactionsUseCase
{
    public async Task<PagedResponse<TransactionResponse>> ExecuteAsync(
        GetAllTransactionsRequest request,
        CancellationToken cancellationToken = default)
    {
        await validator.ValidateOrThrowAsync(instance: request, cancellationToken: cancellationToken);

        (IReadOnlyList<Transaction>? transactions, bool hasNextPage) = await transactionRepository.GetPagedAsync(
            cursor: request.Cursor,
            pageSize: request.PageSize,
            cancellationToken: cancellationToken);

        return TransactionMapper.ToPagedResponse(
            transactions: transactions,
            hasNextPage: hasNextPage);
    }
}
