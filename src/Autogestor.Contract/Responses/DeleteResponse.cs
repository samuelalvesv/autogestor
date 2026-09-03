using System.Runtime.Serialization;

namespace Autogestor.Contract.Responses;

[DataContract]
public sealed record DeleteResponse
{
    [DataMember(Order = 1)]
    public required Guid Id { get; init; }
}
