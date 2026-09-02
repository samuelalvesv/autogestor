using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Autogestor.Contract.Requests.Categories;

[DataContract]
public sealed record CreateCategoryRequest : Request
{
    [DataMember(Order = 2)]
    [Required(ErrorMessage = "Título inválido")]
    [MinLength(3, ErrorMessage = "O título deve conter no mínimo 3 caracteres")]
    [MaxLength(80, ErrorMessage = "O título deve conter no máximo 80 caracteres")]
    public required string Title { get; init; }

    [DataMember(Order = 3)]
    [Required(ErrorMessage = "Descrição inválida")]
    [MinLength(3, ErrorMessage = "A descrição deve conter no mínimo 3 caracteres")]
    [MaxLength(180, ErrorMessage = "A descrição deve conter no máximo 180 caracteres")]
    public required string Description { get; init; }
}
