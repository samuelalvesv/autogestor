using System.Runtime.Serialization;

namespace Autogestor.Contract.Requests.Categories;

[DataContract]
public sealed record UpdateCategoryRequest
{
    [DataMember(Order = 1)]
    public required Guid Id { get; init; }

    [DataMember(Order = 2)]
    public required string Title { get; init; }

    [DataMember(Order = 3)]
    public required string Description { get; init; }
}
