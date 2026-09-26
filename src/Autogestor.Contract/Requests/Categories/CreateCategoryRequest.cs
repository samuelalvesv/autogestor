using System.Runtime.Serialization;

namespace Autogestor.Contract.Requests.Categories;

[DataContract]
public sealed record CreateCategoryRequest
{
    [DataMember(Order = 1)]
    public required string Title { get; init; }

    [DataMember(Order = 2)]
    public required string Description { get; init; }
}
