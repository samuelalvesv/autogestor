using System.Runtime.Serialization;

namespace Autogestor.Contract.Responses.Categories;

[DataContract]
public sealed record CategoryResponse : TenantEntityResponse
{
    [DataMember(Order = 8)]
    public required string Title { get; init; }
    [DataMember(Order = 9)]
    public required string Description { get; init; }
}
