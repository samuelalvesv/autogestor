using System.Runtime.Serialization;

namespace Autogestor.Contract.Requests;

[DataContract]
public abstract record Request
{
    [DataMember(Order = 1)]
    public required Guid UserId { get; init; }
}
