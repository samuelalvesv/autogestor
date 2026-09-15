using System.Runtime.Serialization;
using Autogestor.Contract.Enums;

namespace Autogestor.Contract.Responses.Transactions;

[DataContract]
public sealed record TransactionResponse : TenantEntityResponse
{
    [DataMember(Order = 8)]
    public required string Title { get; init; }

    [DataMember(Order = 9)]
    public required ETransactionType Type { get; init; }

    [DataMember(Order = 10)]
    public required decimal Amount { get; init; }

    [DataMember(Order = 11)]
    public required Guid CategoryId { get; init; }
}
