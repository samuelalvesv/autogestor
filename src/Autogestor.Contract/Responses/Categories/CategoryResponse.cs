using System.Runtime.Serialization;

namespace Autogestor.Contract.Responses.Categories;

[DataContract]
public sealed record CategoryResponse : AuditableEntityResponse
{
    [DataMember(Order = 7)]
    public required string Title { get; init; }
    [DataMember(Order = 8)]
    public required string Description { get; init; }
    [DataMember(Order = 9)]
    public required Guid UserId { get; init; }
}
