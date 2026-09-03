using System.Runtime.Serialization;

namespace Autogestor.Contract.Responses;

[DataContract]
public abstract record AuditableEntityResponse : EntityResponse
{
    [DataMember(Order = 2)]
    public required bool Active { get; init; }
    [DataMember(Order = 3)]
    public required Guid CreatedBy { get; init; }
    [DataMember(Order = 4)]
    public required DateTime CreatedAt { get; init; }
    [DataMember(Order = 5)]
    public required Guid? UpdatedBy { get; init; }
    [DataMember(Order = 6)]
    public required DateTime? UpdatedAt { get; init; }
}
