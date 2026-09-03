using System.Runtime.Serialization;

namespace Autogestor.Contract.Responses;

[DataContract]
public abstract record EntityResponse
{
    [DataMember(Order = 1)]
    public required Guid Id { get; init; }
}
