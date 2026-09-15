using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Autogestor.Contract.Enums;

namespace Autogestor.Contract.Requests.Transactions;

[DataContract]
public sealed record UpdateTransactionRequest
{
    [DataMember(Order = 1)]
    [Required(ErrorMessage = "Transação inválida")]
    public required Guid Id { get; init; }

    [DataMember(Order = 2)]
    [Required(ErrorMessage = "Título inválido")]
    [MinLength(3, ErrorMessage = "O título deve conter no mínimo 3 caracteres")]
    [MaxLength(80, ErrorMessage = "O título deve conter no máximo 80 caracteres")]
    public required string Title { get; init; }

    [DataMember(Order = 3)]
    [Required(ErrorMessage = "Tipo de transação inválido")]
    [EnumDataType(typeof(ETransactionType), ErrorMessage = "Tipo de transação inválido")]
    public required ETransactionType Type { get; init; }

    [DataMember(Order = 4)]
    [Required(ErrorMessage = "Valor inválido")]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
    public required decimal Amount { get; init; }

    [DataMember(Order = 5)]
    [Required(ErrorMessage = "Categoria inválida")]
    public required Guid CategoryId { get; init; }
}
