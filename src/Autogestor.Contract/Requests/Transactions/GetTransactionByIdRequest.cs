using System.Runtime.Serialization;

namespace Autogestor.Contract.Requests.Transactions;

[DataContract]
public sealed record GetTransactionByIdRequest
{
    [DataMember(Order = 1)]
    public required Guid Id { get; init; }
}
