using System.Runtime.Serialization;

namespace Autogestor.Contract.Requests.Categories;

[DataContract]
public sealed record GetCategoryByIdRequest
{
    [DataMember(Order = 1)]
    public required Guid Id { get; init; }
}
