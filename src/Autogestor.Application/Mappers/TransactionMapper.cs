using Autogestor.Contract.Enums;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Transactions;
using Autogestor.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Autogestor.Application.Mappers;

[Mapper]
public static partial class TransactionMapper
{
    [MapperIgnoreSource(nameof(Transaction.Category))]
    public static partial TransactionResponse ToResponse(Transaction transaction);

    public static partial IReadOnlyList<TransactionResponse> ToResponseList(IReadOnlyList<Transaction> transactions);

    public static partial ETransactionType ToContract(Domain.Enums.ETransactionType type);

    public static partial Domain.Enums.ETransactionType ToDomain(ETransactionType type);

    public static PagedResponse<TransactionResponse> ToPagedResponse(
        IReadOnlyList<Transaction>? transactions,
        bool hasNextPage)
    {
        IReadOnlyList<TransactionResponse> items = ToResponseList(transactions: transactions ?? []);

        return new PagedResponse<TransactionResponse>
        {
            Data = items,
            HasNextPage = hasNextPage,
            NextCursor = hasNextPage && items.Count > 0 ? items[^1].Id : null
        };
    }
}
