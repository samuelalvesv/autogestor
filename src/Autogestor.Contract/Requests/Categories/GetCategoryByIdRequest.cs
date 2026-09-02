using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Autogestor.Contract.Requests.Categories;

[DataContract]
public sealed record GetByIdCategoryRequest : Request
{
    [DataMember(Order = 2)]
    [Required(ErrorMessage = "Categoria inválida")]
    public required Guid Id { get; init; }
}
