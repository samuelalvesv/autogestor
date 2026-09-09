using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Autogestor.Contract.Requests.Categories;

[DataContract]
public sealed record DeleteCategoryRequest
{
    [DataMember(Order = 1)]
    [Required(ErrorMessage = "Categoria inválida")]
    public required Guid Id { get; init; }
}
