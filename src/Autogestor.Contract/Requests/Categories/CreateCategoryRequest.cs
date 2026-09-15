using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Autogestor.Contract.Requests.Categories;

[DataContract]
public sealed record CreateCategoryRequest
{
    [DataMember(Order = 1)]
    [Required(ErrorMessage = "Título inválido")]
    [MinLength(length: 3, ErrorMessage = "O título deve conter no mínimo 3 caracteres")]
    [MaxLength(length: 80, ErrorMessage = "O título deve conter no máximo 80 caracteres")]
    public required string Title { get; init; }

    [DataMember(Order = 2)]
    [Required(ErrorMessage = "Descrição inválida")]
    [MinLength(length: 3, ErrorMessage = "A descrição deve conter no mínimo 3 caracteres")]
    [MaxLength(length: 180, ErrorMessage = "A descrição deve conter no máximo 180 caracteres")]
    public required string Description { get; init; }
}
