using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Autogestor.Contract.Requests.Transactions;

[DataContract]
public sealed record GetTransactionByIdRequest
{
    [DataMember(Order = 1)]
    [Required(ErrorMessage = "Transação inválida")]
    public required Guid Id { get; init; }
}
