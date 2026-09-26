using System.Runtime.Serialization;
using Autogestor.Contract.Enums;

namespace Autogestor.Contract.Requests.Transactions;

[DataContract]
public sealed record UpdateTransactionRequest
{
    [DataMember(Order = 1)]
    public required Guid Id { get; init; }

    [DataMember(Order = 2)]
    public required string Title { get; init; }

    [DataMember(Order = 3)]
    public required ETransactionType Type { get; init; }

    [DataMember(Order = 4)]
    public required decimal Amount { get; init; }

    [DataMember(Order = 5)]
    public required Guid CategoryId { get; init; }
}
