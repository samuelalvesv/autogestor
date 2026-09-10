using System.Runtime.Serialization;

namespace Autogestor.Contract.Responses;

[DataContract]
public abstract record TenantEntityResponse : AuditableEntityResponse
{
    [DataMember(Order = 7)]
    public required Guid TenantId { get; init; }
}
